using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common.Collections;
using iLynx.Common.Configuration;
using iLynx.Common;
using iLynx.Common.Threading;
using iLynx.Common.Threading.Unmanaged;
using iLynx.Common.WPF;

namespace LMaML.Services
{
    public class NonManagingPlayerService : ComponentBase, IPlayerService
    {
        private const int MaxRecursion = 25;
        private readonly IPlaylistService playlistService;
        private readonly IAudioPlayer player;
        private readonly IPublicTransport publicTransport;
        private readonly IConfigurationManager configurationManager;
        private readonly IGlobalHotkeyService hotkeyService;
        private readonly IConfigurableValue<int> prebufferSongs;
        protected readonly IConfigurableValue<double> PlayNextThreshold;
        protected readonly IConfigurableValue<double> TrackInterchangeCrossfadeTime;
        protected readonly IConfigurableValue<int> TrackInterchangeCrossFadeSteps;
        private readonly IConfigurableValue<int> maxBackStack;
        private readonly List<TrackContainer> preBuffered;
        private readonly List<TrackContainer> backStack;

        private PlayingState state;
        protected TrackContainer CurrentTrack;
        protected DateTime LastProgress = DateTime.Now;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBase" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="playlistService">The playlist service.</param>
        /// <param name="player">The player.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="hotkeyService">The hotkey service.</param>
        public NonManagingPlayerService(ILogger logger,
            IPlaylistService playlistService,
            IAudioPlayer player,
            IPublicTransport publicTransport,
            IConfigurationManager configurationManager,
            IGlobalHotkeyService hotkeyService)
            : base(logger)
        {
            playlistService.Guard("playlistService");
            player.Guard("player");
            publicTransport.Guard("publicTransport");
            configurationManager.Guard("configurationManager");
            state = PlayingState.Stopped;
            this.playlistService = playlistService;
            this.player = player;
            this.publicTransport = publicTransport;
            this.configurationManager = configurationManager;
            this.hotkeyService = hotkeyService;
            publicTransport.ApplicationEventBus.Subscribe<PlaylistUpdatedEvent>(OnPlaylistUpdated);
            publicTransport.ApplicationEventBus.Subscribe<ShuffleChangedEvent>(OnShuffleChanged);
            prebufferSongs = configurationManager.GetValue("PlayerService.PrebufferSongs", 2);
            PlayNextThreshold = configurationManager.GetValue("PlayerService.PlayNextThreshnoldMs", 500d);
            TrackInterchangeCrossfadeTime = configurationManager.GetValue("PlayerService.TrackInterchangeCrossfadeTimeMs", 500d);
            TrackInterchangeCrossFadeSteps = configurationManager.GetValue("PlayerService.TrackInterchangeCrossfadeSteps", 50);
            maxBackStack = configurationManager.GetValue("PlayerService.MaxBackStack", 2000);
            preBuffered = new List<TrackContainer>(prebufferSongs.Value);
            backStack = new List<TrackContainer>(maxBackStack.Value);
            RegisterHotkeys();
        }

        protected void RegisterHotkeys()
        {
            var playPauseValue = configurationManager.GetValue("Play/Pause", new HotkeyDescriptor(ModifierKeys.None, Key.MediaPlayPause), KnownConfigSections.GlobalHotkeys);
            playPauseValue.ValueChanged += PlayPauseValueOnValueChanged;
            var nextValue = configurationManager.GetValue("Next", new HotkeyDescriptor(ModifierKeys.None, Key.MediaNextTrack), KnownConfigSections.GlobalHotkeys);
            nextValue.ValueChanged += NextValueOnValueChanged;
            var previousValue = configurationManager.GetValue("Previous", new HotkeyDescriptor(ModifierKeys.None, Key.MediaPreviousTrack), KnownConfigSections.GlobalHotkeys);
            previousValue.ValueChanged += PreviousValueOnValueChanged;
            var stopValue = configurationManager.GetValue("Stop", new HotkeyDescriptor(ModifierKeys.None, Key.MediaStop), KnownConfigSections.GlobalHotkeys);
            stopValue.ValueChanged += StopValueOnValueChanged;
            hotkeyService.RegisterHotkey(playPauseValue.Value, PlayPause); // Closing in on a train wreck...
            hotkeyService.RegisterHotkey(nextValue.Value, Next);
            hotkeyService.RegisterHotkey(previousValue.Value, Previous);
            hotkeyService.RegisterHotkey(stopValue.Value, Stop);
        }

        private void StopValueOnValueChanged(object sender, ValueChangedEventArgs<object> valueChangedEventArgs)
        {
            hotkeyService.ReRegisterHotkey(valueChangedEventArgs.OldValue as HotkeyDescriptor, valueChangedEventArgs.NewValue as HotkeyDescriptor, Stop);
        }

        private void PreviousValueOnValueChanged(object sender, ValueChangedEventArgs<object> valueChangedEventArgs)
        {
            hotkeyService.ReRegisterHotkey(valueChangedEventArgs.OldValue as HotkeyDescriptor, valueChangedEventArgs.NewValue as HotkeyDescriptor, Previous);
        }

        private void NextValueOnValueChanged(object sender, ValueChangedEventArgs<object> valueChangedEventArgs)
        {
            hotkeyService.ReRegisterHotkey(valueChangedEventArgs.OldValue as HotkeyDescriptor, valueChangedEventArgs.NewValue as HotkeyDescriptor, Next);
        }

        private void PlayPauseValueOnValueChanged(object sender, ValueChangedEventArgs<object> valueChangedEventArgs)
        {
            hotkeyService.ReRegisterHotkey(valueChangedEventArgs.OldValue as HotkeyDescriptor, valueChangedEventArgs.NewValue as HotkeyDescriptor, PlayPause);
        }

        protected virtual void OnShuffleChanged(ShuffleChangedEvent shuffleChangedEvent)
        {
            ReBuffer();
        }

        protected virtual void OnPlaylistUpdated(PlaylistUpdatedEvent e)
        {
            ReBuffer();
        }

        protected void SendProgress()
        {
            publicTransport.ApplicationEventBus.Send(new TrackProgressEvent(CurrentTrack.CurrentPositionMillisecond, CurrentTrack.CurrentProgress));
            LastProgress = DateTime.Now;
        }

        /// <summary>
        /// Seeks to the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Seek(TimeSpan offset)
        {
            Seek(offset.TotalMilliseconds);
        }

        protected void UpdateState()
        {
            State = CurrentTrack == null
                        ? PlayingState.Stopped
                        : CurrentTrack.IsPaused
                              ? PlayingState.Paused
                              : CurrentTrack.IsPlaying
                                    ? PlayingState.Playing
                                    : PlayingState.Stopped;
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public PlayingState State
        {
            get { return state; }
            private set
            {
                state = value;
                publicTransport.ApplicationEventBus.Send(new PlayingStateChangedEvent(value));
            }
        }

        /// <summary>
        /// Currents the channel as readonly.
        /// </summary>
        /// <value>
        /// The current channel as readonly.
        /// </value>
        /// <returns></returns>
        public ITrack CurrentTrackAsReadonly
        {
            get { return CurrentTrack.AsReadonly; }
        }

        /// <summary>
        /// Plays this instance.
        /// </summary>
        /// <param name="file"></param>
        public virtual void Play(StorableTaggedFile file)
        {
            file.Guard("file");
            var oldCurrent = CurrentTrack;
            var newChannel = new TrackContainer(player, file);
            try
            {
                newChannel.Preload();
            }
            catch (Exception e)
            {
                newChannel.Dispose();
                LogException(e, MethodBase.GetCurrentMethod());
                LogWarning("File Was: {0}", file.Filename);
                return;
            }
            SwapChannels(newChannel);
            
            if (null != oldCurrent)
                PushContainer(oldCurrent);

            var index = playlistService.Files.IndexOf(file);
            if (index < 0) return;
            playlistService.SetPlaylistIndex(file);
            ReBuffer();
        }

        public virtual void Play(ITrack track)
        {
            var oldCurrent = CurrentTrack;
            var newChannel = new TrackContainer(track);
            try
            {
                newChannel.Preload();
            }
            catch (Exception e)
            {
                newChannel.Dispose();
                LogException(e, MethodBase.GetCurrentMethod());
                LogWarning("Track Was: {0}", track);
                return;
            }
            SwapChannels(newChannel);

            if (null != oldCurrent)
                PushContainer(oldCurrent);

            playlistService.SetPlaylistIndex(null);
            ReBuffer();
        }

        /// <summary>
        /// Seeks the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public virtual void Seek(double offset)
        {
            if (null == CurrentTrack) return;
            CurrentTrack.Seek(offset);
        }

        /// <summary>
        /// Plays the pause.
        /// </summary>
        public virtual void PlayPause()
        {
            if (null == CurrentTrack)
            {
                var i = 0;
                DoTheNextOne(ref i);
                return;
            }
            if (CurrentTrack.IsPaused)
                CurrentTrack.Play(1f);
            else
                CurrentTrack.Pause();
            UpdateState();
        }

        /// <summary>
        /// Nexts this instance.
        /// </summary>
        public virtual void Next()
        {
            var i = 0;
            DoTheNextOne(ref i);
        }

        /// <summary>
        /// Notifies the new track.
        /// </summary>
        /// <param name="file">The file.</param>
        private void NotifyNewTrack(TrackContainer file)
        {
            publicTransport.ApplicationEventBus.Send(new TrackChangedEvent(file.File, file.Length));
        }

        protected void DoTheNextOne(ref int recursion)
        {
            var oldCurrent = CurrentTrack;
            ++recursion;
            if (recursion >= MaxRecursion) return;
            if (!SwapToNext())
            {
                PreBufferNext();
                DoTheNextOne(ref recursion);
                return;
            }
            if (null != oldCurrent)
                PushContainer(oldCurrent);
            PreBufferNext();
        }

        /// <summary>
        /// Swaps to next.
        /// </summary>
        private bool SwapToNext()
        {
            if (preBuffered.Count < 1)
                PreBufferNext();
            if (preBuffered.Count < 1) return false; // Nobody here but us chickens
            var newChannel = preBuffered[0];
            preBuffered.RemoveAt(0);
            return SwapChannels(newChannel);
        }

        /// <summary>
        /// Pushes the current.
        /// </summary>
        private void PushContainer(TrackContainer container)
        {
            if (null == CurrentTrack) return;
            if (backStack.Count >= maxBackStack.Value)
            {
                for (var i = 0; i < 100 / maxBackStack.Value * 10; ++i)
                    backStack.RemoveAt(0);
            }
            TrimBackBuffered();
            backStack.Add(container);
        }

        /// <summary>
        /// Disposes the reload.
        /// </summary>
        private void TrimBackBuffered()
        {
            if (backStack.Count <= prebufferSongs.Value) return;
            for (var i = 0; i < backStack.Count - prebufferSongs.Value; ++i)
                backStack[i].Dispose();
            for (var i = backStack.Count - prebufferSongs.Value; i < backStack.Count; ++i)
                backStack[i].Preload();
        }

        /// <summary>
        /// Swaps the channels.
        /// </summary>
        /// <param name="nextTrack">The next channel.</param>
        /// <returns></returns>
        private bool SwapChannels(TrackContainer nextTrack)
        {
            try { nextTrack.Play(0f); }
            catch (Exception e)
            {
                nextTrack.Dispose();
                LogException(e, MethodBase.GetCurrentMethod());
                return false;
            }
            if (null != CurrentTrack)
            {
                CrossFade(CurrentTrack, nextTrack);
            }
            else
                FadeIn(nextTrack);
            CurrentTrack = nextTrack;
            NotifyNewTrack(CurrentTrack);
            UpdateState();
            return true;
        }

        protected virtual void FadeIn(ITrack track)
        {
            if (null == track) return;
            var steps = TrackInterchangeCrossFadeSteps.Value;
            var interval = TimeSpan.FromMilliseconds(TrackInterchangeCrossfadeTime.Value / steps);
            var toStepSize = (1f - track.Volume) / steps;
            for (var i = 0; i < steps; ++i)
            {
                track.Volume += toStepSize;
                Thread.CurrentThread.Join(interval);
            }
        }

        protected virtual void CrossFade(ITrack from, ITrack to)
        {
            var steps = TrackInterchangeCrossFadeSteps.Value;
            var interval = TimeSpan.FromMilliseconds(TrackInterchangeCrossfadeTime.Value / steps);
            var fromStepSize = from.Volume / steps;
            var toStepSize = (1f - to.Volume) / steps;
            for (var i = 0; i < steps; ++i)
            {
                from.Volume -= fromStepSize;
                to.Volume += toStepSize;
                Thread.CurrentThread.Join(interval);
            }
            from.Stop();
        }

        /// <summary>
        /// Res the buffer.
        /// </summary>
        private void ReBuffer()
        {
            foreach (var container in preBuffered)
                container.Dispose();
            preBuffered.Clear();
            if (!playlistService.Shuffle && null != CurrentTrack)
                playlistService.SetPlaylistIndex(CurrentTrack.File);
            PreBufferNext();
        }

        /// <summary>
        /// Pres the buffer next.
        /// </summary>
        private void PreBufferNext()
        {
            var errorCount = 0;
            while (preBuffered.Count < prebufferSongs.Value)
            {
                var next = playlistService.Next();
                if (null == next) break;
                var container = new TrackContainer(player, next);
                try { container.Preload(); }
                catch (Exception e)
                {
                    container.Dispose();
                    ++errorCount;
                    LogException(e, MethodBase.GetCurrentMethod());
                    LogWarning("File Was: {0}", container.File.Filename);
                    if (errorCount >= 50)
                    {
                        LogCritical("Too many errors while prebuffering, giving up...");
                        break;
                    }
                    continue;
                }
                preBuffered.Add(container);
                LogInformation("PlayerService has {0} files PreBuffered", preBuffered.Count);
            }
        }

        /// <summary>
        /// Gets the FFT.
        /// </summary>
        /// <returns></returns>
        public virtual float[] FFT(out float sampleRate, int size = 64)
        {
            var rate = null == CurrentTrack ? 0f : CurrentTrack.SampleRate;
            var fft = null == CurrentTrack ? new float[size] : CurrentTrack.FFTStereo(size);
            sampleRate = rate;
            return fft;
        }

        /// <summary>
        /// Previouses this instance.
        /// </summary>
        public virtual void Previous()
        {
            if (backStack.Count < 1) return; // Nobody here but us chickens
            var channel = backStack[backStack.Count - 1];
            backStack.RemoveAt(backStack.Count - 1);
            var oldCurrent = CurrentTrack;
            SwapChannels(channel);
            preBuffered.Insert(0, oldCurrent);
            TrimBackBuffered();
            ReBuffer();
            if (null != oldCurrent)
                oldCurrent.Dispose();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public virtual void Stop()
        {
            if (null == CurrentTrack) return;
            CurrentTrack.Stop();
            UpdateState();
            SendProgress();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            
        }
    }
    /// <summary>
    /// PlayerService
    /// </summary>
    public class PlayerService : NonManagingPlayerService
    {
        private bool doMange = true;
        private readonly IPriorityQueue<Action> managerQueue = new PriorityQueue<Action>();
        private readonly IWorker managerThread;
        private CancellationToken token;
        private CancellationTokenSource tokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerService" /> class.
        /// </summary>
        /// <param name="playlistService">The playlist service.</param>
        /// <param name="player">The player.</param>
        /// <param name="threadManager">The thread manager service.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="hotkeyService">The hotkey service.</param>
        /// <param name="logger">The logger.</param>
        public PlayerService(IPlaylistService playlistService,
            IAudioPlayer player,
            IThreadManager threadManager,
            IPublicTransport publicTransport,
            IConfigurationManager configurationManager,
            IGlobalHotkeyService hotkeyService,
            ILogger logger)
            : base(logger, playlistService, player, publicTransport, configurationManager, hotkeyService)
        {
            threadManager.Guard("threadManagerService");
            managerThread = threadManager.StartNew(Manage);
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
        }

        public override void Dispose()
        {
            doMange = false;
            managerThread.Wait();
        }

        /// <summary>
        /// Manages this instance.
        /// </summary>
        private void Manage()
        {
            while (doMange)
            {
                Thread.CurrentThread.Join(1);
                Action a;
                if (null != (a = managerQueue.RawDequeue()))
                    a();
                if (null != CurrentTrack && (CurrentTrack.Length.TotalMilliseconds - CurrentTrack.CurrentPositionMillisecond) <= PlayNextThreshold.Value)
                {
                    var pre = 0;
                    DoTheNextOne(ref pre);
                }
                if (PlayingState.Playing != State || DateTime.Now - LastProgress < progressUpdateInterval || null == CurrentTrack)
                    continue;
                SendProgress();

            }
            if (null == CurrentTrack) return;
            CurrentTrack.Dispose();
            CurrentTrack.Stop();
        }

        readonly TimeSpan progressUpdateInterval = TimeSpan.FromMilliseconds(100d);

        public override void Next()
        {
            managerQueue.Enqueue(base.Next);
        }

        public override float[] FFT(out float sampleRate, int size = 64)
        {
            float[] result = null;
            var rate = 0f;
            managerQueue.Enqueue(() => { result = base.FFT(out rate, size); });
            while (null == result && doMange)
                Thread.CurrentThread.Join(1);
            sampleRate = rate;
            return result;
        }

        private Task crossFader;

        protected override void FadeIn(ITrack track)
        {
            CancelFade();
            crossFader = Task.Factory.StartNew(() =>
            {
                if (null == track) return;
                var steps = TrackInterchangeCrossFadeSteps.Value;
                var interval = TimeSpan.FromMilliseconds(TrackInterchangeCrossfadeTime.Value/steps);
                var toStepSize = (1f - track.Volume)/steps;
                for (var i = 0; i < steps; ++i)
                {
                    managerQueue.Enqueue(() => track.Volume += toStepSize);
                    if (token.IsCancellationRequested)
                        break;
                    Thread.CurrentThread.Join(interval);
                }
            }, token);
        }

        private void CancelFade()
        {
            if (null == crossFader) return;
            tokenSource.Cancel();
            crossFader.Wait();
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
        }

        protected override void CrossFade(ITrack from, ITrack to)
        {
            CancelFade();
            crossFader = Task.Factory.StartNew(() =>
            {
                var steps = TrackInterchangeCrossFadeSteps.Value;
                var interval = TimeSpan.FromMilliseconds(TrackInterchangeCrossfadeTime.Value / steps);
                var fromStepSize = from.Volume / steps;
                var toStepSize = (1f - to.Volume) / steps;
                for (var i = 0; i < steps; ++i)
                {
                    managerQueue.Enqueue(() =>
                    {
                        from.Volume -= fromStepSize;
                        to.Volume += toStepSize;
                    });
                    if (token.IsCancellationRequested)
                        break;
                    Thread.CurrentThread.Join(interval);
                }
                from.Stop();
            }, token);
        }

        public override void Play(StorableTaggedFile file)
        {
            managerQueue.Enqueue(() => base.Play(file));
        }

        public override void PlayPause()
        {
            managerQueue.Enqueue(base.PlayPause);
        }

        public override void Previous()
        {
            managerQueue.Enqueue(base.Previous);
        }

        public override void Seek(double offset)
        {
            managerQueue.Enqueue(() => base.Seek(offset));
        }

        public override void Stop()
        {
            managerQueue.Enqueue(base.Stop);
        }

        public override void Play(ITrack track)
        {
            managerQueue.Enqueue(() => base.Play(track));
        }
    }
}

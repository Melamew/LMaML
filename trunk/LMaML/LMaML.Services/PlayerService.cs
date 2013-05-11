using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common.Configuration;
using iLynx.Common;
using iLynx.Common.Threading;
using iLynx.Common.Threading.Unmanaged;

namespace LMaML.Services
{
    /// <summary>
    /// PlayerService
    /// </summary>
    public class PlayerService : ComponentBase, IPlayerService
    {
        private readonly IPlaylistService playlistService;
        private readonly IAudioPlayer player;
        private readonly IPublicTransport publicTransport;
        private readonly IConfigurationManager configurationManager;
        private readonly IGlobalHotkeyService hotkeyService;
        private readonly IConfigurableValue<int> prebufferSongs;
        private readonly IConfigurableValue<double> playNextThreshold;
        private readonly IConfigurableValue<double> trackInterchangeCrossfadeTime;
        private readonly IConfigurableValue<int> trackInterchangeCrossFadeSteps;
        private readonly IConfigurableValue<int> maxBackStack;
        private readonly List<ChannelContainer> preBuffered;
        private ChannelContainer currentChannel;
        private bool doMange = true;
        private readonly ConcurrentQueue<Action> managerQueue = new ConcurrentQueue<Action>();
        private readonly List<ChannelContainer> backStack;
        private readonly IWorker managerThread;

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
            : base(logger)
        {
            playlistService.Guard("playlistService");
            player.Guard("player");
            threadManager.Guard("threadManagerService");
            publicTransport.Guard("publicTransport");
            configurationManager.Guard("configurationManager");
            state = PlayingState.Stopped;
            this.playlistService = playlistService;
            this.player = player;
            this.publicTransport = publicTransport;
            this.configurationManager = configurationManager;
            this.hotkeyService = hotkeyService;
            managerThread = threadManager.StartNew(Manage);
            publicTransport.ApplicationEventBus.Subscribe<PlaylistUpdatedEvent>(OnPlaylistUpdated);
            publicTransport.ApplicationEventBus.Subscribe<ShuffleChangedEvent>(OnShuffleChanged);
            prebufferSongs = configurationManager.GetValue("PlayerService.PrebufferSongs", 2);
            playNextThreshold = configurationManager.GetValue("PlayerService.PlayNextThreshnoldPercent", 98.0d);
            trackInterchangeCrossfadeTime = configurationManager.GetValue("PlayerService.TrackInterchangeCrossfadeTimeMs", 250d);
            trackInterchangeCrossFadeSteps = configurationManager.GetValue("PlayerService.TrackInterchangeCrossfadeSteps", 50);
            maxBackStack = configurationManager.GetValue("PlayerService.MaxBackStack", 2000);
            preBuffered = new List<ChannelContainer>(prebufferSongs.Value);
            backStack = new List<ChannelContainer>(maxBackStack.Value);
            RegisterHotkeys();
        }

        private void RegisterHotkeys()
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

        private void StopValueOnValueChanged(object sender, ValueChangedEventArgs<HotkeyDescriptor> valueChangedEventArgs)
        {
            hotkeyService.UnregisterHotkey(valueChangedEventArgs.OldValue, Stop);
            hotkeyService.RegisterHotkey(valueChangedEventArgs.NewValue, Stop);
        }

        private void PreviousValueOnValueChanged(object sender, ValueChangedEventArgs<HotkeyDescriptor> valueChangedEventArgs)
        {
            hotkeyService.UnregisterHotkey(valueChangedEventArgs.OldValue, Previous);
            hotkeyService.RegisterHotkey(valueChangedEventArgs.NewValue, Previous);
        }

        private void NextValueOnValueChanged(object sender, ValueChangedEventArgs<HotkeyDescriptor> valueChangedEventArgs)
        {
            hotkeyService.UnregisterHotkey(valueChangedEventArgs.OldValue, Next);
            hotkeyService.RegisterHotkey(valueChangedEventArgs.NewValue, Next);
        }

        private void PlayPauseValueOnValueChanged(object sender, ValueChangedEventArgs<HotkeyDescriptor> valueChangedEventArgs)
        {
            hotkeyService.UnregisterHotkey(valueChangedEventArgs.OldValue, PlayPause);
            hotkeyService.RegisterHotkey(valueChangedEventArgs.NewValue, PlayPause);
        }

        private void OnShuffleChanged(ShuffleChangedEvent shuffleChangedEvent)
        {
            managerQueue.Enqueue(ReBuffer);
        }

        private void OnPlaylistUpdated(PlaylistUpdatedEvent e)
        {
            managerQueue.Enqueue(ReBuffer);
        }

        private DateTime lastProgress = DateTime.Now;

        /// <summary>
        /// Manages this instance.
        /// </summary>
        private void Manage()
        {
            while (doMange)
            {
                Thread.CurrentThread.Join(1);
                Action a;
                if (managerQueue.TryDequeue(out a))
                    a();
                if (null != currentChannel && currentChannel.CurrentProgress >= playNextThreshold.Value)
                {
                    var pre = 0;
                    DoTheNextOne(ref pre);
                }
                if (PlayingState.Playing != state || DateTime.Now - lastProgress < t250Ms || null == currentChannel)
                    continue;
                SendProgress();

            }
            if (null == currentChannel) return;
            currentChannel.Dispose();
            currentChannel.Stop();
        }

        readonly TimeSpan t250Ms = TimeSpan.FromMilliseconds(250d);
        private void SendProgress()
        {
            publicTransport.ApplicationEventBus.Send(new TrackProgressEvent(currentChannel.CurrentPosition, currentChannel.CurrentProgress));
            lastProgress = DateTime.Now;
        }

        /// <summary>
        /// Seeks to the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Seek(TimeSpan offset)
        {
            Seek(offset.TotalMilliseconds);
        }

        /// <summary>
        /// Seeks the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Seek(double offset)
        {
            managerQueue.Enqueue(() => DoSeek(offset));
        }

        private void DoSeek(double offset)
        {
            if (null == currentChannel) return;
            currentChannel.Seek(offset);
        }

        /// <summary>
        /// Plays this instance.
        /// </summary>
        /// <param name="file"></param>
        public void Play(StorableTaggedFile file)
        {
            managerQueue.Enqueue(() => DoPlay(file));
        }

        private void DoPlay(StorableTaggedFile file)
        {
            file.Guard("file");
            var oldCurrent = currentChannel;
            var newChannel = new ChannelContainer(player, file);
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
            if (null == oldCurrent) return;
            PushContainer(oldCurrent);
        }

        /// <summary>
        /// Plays the pause.
        /// </summary>
        public void PlayPause()
        {
            managerQueue.Enqueue(DoPlayPause);
        }

        /// <summary>
        /// Does the play pause.
        /// </summary>
        private void DoPlayPause()
        {
            if (null == currentChannel) return;
            if (currentChannel.IsPaused)
                currentChannel.Play(100f);
            else
                currentChannel.Pause();
            UpdateState();
        }

        private void UpdateState()
        {
            State = currentChannel == null
                ? PlayingState.Stopped
                : currentChannel.IsPaused
                    ? PlayingState.Paused
                    : currentChannel.IsPlaying
                        ? PlayingState.Playing
                        : PlayingState.Stopped;
        }

        /// <summary>
        /// Nexts this instance.
        /// </summary>
        public void Next()
        {
            managerQueue.Enqueue(() =>
                                     {
                                         var i = 0;
                                         DoTheNextOne(ref i);
                                     });
        }

        /// <summary>
        /// Notifies the new track.
        /// </summary>
        /// <param name="file">The file.</param>
        private void NotifyNewTrack(ChannelContainer file)
        {
            publicTransport.ApplicationEventBus.Send(new TrackChangedEvent(file.File, file.Length));
        }

        private const int MaxRecursion = 25;
        private void DoTheNextOne(ref int recursion)
        {
            var oldCurrent = currentChannel;
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
        private void PushContainer(ChannelContainer container)
        {
            if (null == currentChannel) return;
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
        /// <param name="nextChannel">The next channel.</param>
        /// <returns></returns>
        private bool SwapChannels(ChannelContainer nextChannel)
        {
            try { nextChannel.Play(0f); }
            catch (Exception e)
            {
                nextChannel.Dispose();
                LogException(e, MethodBase.GetCurrentMethod());
                return false;
            }
            if (null != currentChannel)
            {
                CrossFade(currentChannel, nextChannel);
                currentChannel.Stop();
            }
            else
                nextChannel.FadeIn(TimeSpan.FromMilliseconds(trackInterchangeCrossfadeTime.Value));
            currentChannel = nextChannel;
            NotifyNewTrack(currentChannel);
            UpdateState();
            return true;
        }

        private void CrossFade(IChannel from, IChannel to)
        {
            var steps = trackInterchangeCrossFadeSteps.Value;
            var interval = TimeSpan.FromMilliseconds(trackInterchangeCrossfadeTime.Value / steps);
            var fromStepSize = from.Volume / steps;
            var toStepSize = (100f - to.Volume) / steps;
            for (var i = 0; i < steps; ++i)
            {
                from.Volume -= fromStepSize;
                to.Volume += toStepSize;
                Thread.CurrentThread.Join(interval);
            }
        }

        /// <summary>
        /// Res the buffer.
        /// </summary>
        private void ReBuffer()
        {
            foreach (var container in preBuffered)
                container.Dispose();
            preBuffered.Clear();
            PreBufferNext();
        }

        /// <summary>
        /// Pres the buffer next.
        /// </summary>
        private void PreBufferNext()
        {
            while (preBuffered.Count < prebufferSongs.Value)
            {
                var next = playlistService.Next();
                if (null == next) break;
                var container = new ChannelContainer(player, next);
                try { container.Preload(); }
                catch (Exception e)
                {
                    container.Dispose();
                    LogException(e, MethodBase.GetCurrentMethod());
                    LogWarning("File Was: {0}", container.File.Filename);
                }
                preBuffered.Add(container);
            }
        }

        /// <summary>
        /// Gets the FFT.
        /// </summary>
        /// <returns></returns>
        public float[] FFT(out float sampleRate, int size = 64)
        {
            float[] fft = null;
            var rate = 0f;
            managerQueue.Enqueue(() =>
                                     {
                                         rate = null == currentChannel ? 0f : currentChannel.SampleRate;
                                         fft = null == currentChannel ? new float[size] : currentChannel.FFTStereo(size);
                                     });
            while (fft == null && doMange) Thread.CurrentThread.Join(1);
            sampleRate = rate;
            return fft;
        }

        /// <summary>
        /// Previouses this instance.
        /// </summary>
        public void Previous()
        {
            managerQueue.Enqueue(DoPrevious);
        }

        private void DoPrevious()
        {
            if (backStack.Count < 1) return; // Nobody here but us chickens
            var channel = backStack[backStack.Count - 1];
            backStack.RemoveAt(backStack.Count - 1);
            var oldCurrent = currentChannel;
            SwapChannels(channel);
            // Enqueue this in the preBuffered items instead of pushing it to the backstack
            TrimBackBuffered();
            ReBuffer();
            if (null != oldCurrent)
                oldCurrent.Dispose();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            managerQueue.Enqueue(DoStop);
        }

        /// <summary>
        /// Does the stop.
        /// </summary>
        private void DoStop()
        {
            if (null == currentChannel) return;
            currentChannel.Stop();
            UpdateState();
            SendProgress();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            doMange = false;
            managerThread.Wait();
        }

        private PlayingState state;
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
        public IChannel CurrentChannelAsReadonly
        {
            get { return currentChannel == null ? null : currentChannel.AsReadonly; }
        }
    }
}

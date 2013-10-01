﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
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
        private readonly List<TrackContainer> preBuffered;
        private TrackContainer currentTrack;
        private bool doMange = true;
        private readonly IPriorityQueue<Action> managerQueue = new PriorityQueue<Action>();
        private readonly List<TrackContainer> backStack;
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
            playNextThreshold = configurationManager.GetValue("PlayerService.PlayNextThreshnoldMs", 500d);
            trackInterchangeCrossfadeTime = configurationManager.GetValue("PlayerService.TrackInterchangeCrossfadeTimeMs", 250d);
            trackInterchangeCrossFadeSteps = configurationManager.GetValue("PlayerService.TrackInterchangeCrossfadeSteps", 50);
            maxBackStack = configurationManager.GetValue("PlayerService.MaxBackStack", 2000);
            preBuffered = new List<TrackContainer>(prebufferSongs.Value);
            backStack = new List<TrackContainer>(maxBackStack.Value);
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
                if (null != (a = managerQueue.RawDequeue()))
                    a();
                if (null != currentTrack && (currentTrack.Length.TotalMilliseconds - currentTrack.CurrentPositionMillisecond) <= playNextThreshold.Value)
                {
                    var pre = 0;
                    DoTheNextOne(ref pre);
                }
                if (PlayingState.Playing != state || DateTime.Now - lastProgress < t250Ms || null == currentTrack)
                    continue;
                SendProgress();

            }
            if (null == currentTrack) return;
            currentTrack.Dispose();
            currentTrack.Stop();
        }

        readonly TimeSpan t250Ms = TimeSpan.FromMilliseconds(250d);
        private void SendProgress()
        {
            publicTransport.ApplicationEventBus.Send(new TrackProgressEvent(currentTrack.CurrentPositionMillisecond, currentTrack.CurrentProgress));
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
            if (null == currentTrack) return;
            currentTrack.Seek(offset);
        }

        /// <summary>
        /// Plays this instance.
        /// </summary>
        /// <param name="file"></param>
        public void Play(StorableTaggedFile file)
        {
            managerQueue.Enqueue(() => DoPlay(file));
            var index = playlistService.Files.IndexOf(file);
            if (index < 0) return;
            managerQueue.Enqueue(() => playlistService.SetPlaylistIndex(file));
            managerQueue.Enqueue(ReBuffer);
        }

        private void DoPlay(StorableTaggedFile file)
        {
            file.Guard("file");
            var oldCurrent = currentTrack;
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
            if (null == currentTrack)
            {
                var i = 0;
                DoTheNextOne(ref i);
                return;
            }
            if (currentTrack.IsPaused)
                currentTrack.Play(100f);
            else
                currentTrack.Pause();
            UpdateState();
        }

        private void UpdateState()
        {
            State = currentTrack == null
                ? PlayingState.Stopped
                : currentTrack.IsPaused
                    ? PlayingState.Paused
                    : currentTrack.IsPlaying
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
        private void NotifyNewTrack(TrackContainer file)
        {
            publicTransport.ApplicationEventBus.Send(new TrackChangedEvent(file.File, file.Length));
        }

        private const int MaxRecursion = 25;
        private void DoTheNextOne(ref int recursion)
        {
            var oldCurrent = currentTrack;
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
            if (null == currentTrack) return;
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
            if (null != currentTrack)
            {
                CrossFade(currentTrack, nextTrack);
            }
            else
                managerQueue.Enqueue(() => nextTrack.FadeIn(TimeSpan.FromMilliseconds(trackInterchangeCrossfadeTime.Value)), Priority.Low);
            currentTrack = nextTrack;
            NotifyNewTrack(currentTrack);
            UpdateState();
            return true;
        }

        private void CrossFade(ITrack from, ITrack to)
        {
            var steps = trackInterchangeCrossFadeSteps.Value;
            var interval = TimeSpan.FromMilliseconds(trackInterchangeCrossfadeTime.Value / steps);
            var fromStepSize = from.Volume / steps;
            var toStepSize = (100f - to.Volume) / steps;
            for (var i = 0; i < steps; ++i)
            {
                managerQueue.Enqueue(() =>
                {
                    from.Volume -= fromStepSize;
                    to.Volume += toStepSize;
                    Thread.CurrentThread.Join(interval);
                }, Priority.Low);
            }
            managerQueue.Enqueue(from.Stop);
        }

        /// <summary>
        /// Res the buffer.
        /// </summary>
        private void ReBuffer()
        {
            foreach (var container in preBuffered)
                container.Dispose();
            preBuffered.Clear();
            if (!playlistService.Shuffle && null != currentTrack)
                playlistService.SetPlaylistIndex(currentTrack.File);
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
        public float[] FFT(out float sampleRate, int size = 64)
        {
            float[] fft = null;
            var rate = 0f;
            managerQueue.Enqueue(() =>
                                     {
                                         rate = null == currentTrack ? 0f : currentTrack.SampleRate;
                                         fft = null == currentTrack ? new float[size] : currentTrack.FFTStereo(size);
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
            var oldCurrent = currentTrack;
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
        public void Stop()
        {
            managerQueue.Enqueue(DoStop);
        }

        /// <summary>
        /// Does the stop.
        /// </summary>
        private void DoStop()
        {
            if (null == currentTrack) return;
            currentTrack.Stop();
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
        public ITrack CurrentTrackAsReadonly
        {
            get { return currentTrack == null ? null : currentTrack.AsReadonly; }
        }
    }
}
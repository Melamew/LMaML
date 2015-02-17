using System;
using System.Threading;
using System.Windows.Input;
using iLynx.Configuration;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Commands;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;
using iLynx.Common.WPF;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace LMaML.PlayerControls.ViewModels
{
    /// <summary>
    /// PlayerControlsViewModel
    /// </summary>
    public class PlayerControlsViewModel : NotificationBase
    {
        private readonly IPlaylistService playlistService;
        private readonly IPublicTransport publicTransport;
        private readonly IDispatcher dispatcher;
        private ICommand playPauseCommand;
        private ICommand stopCommand;
        private ICommand previousCommand;
        private ICommand nextCommand;
        private PlayingState state;
        private readonly Timer seekTimer;
        private bool hasSought;
        private readonly IConfigurableValue<float> volumeValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerControlsViewModel" /> class.
        /// </summary>
        /// <param name="configurationManager"></param>
        /// <param name="playlistService">The playlist service.</param>æ
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        public PlayerControlsViewModel(IConfigurationManager configurationManager, IPlaylistService playlistService, IPublicTransport publicTransport, IDispatcher dispatcher)
        {
            publicTransport.Guard("publicTransport");
            dispatcher.Guard("dispatcher");
            publicTransport.ApplicationEventBus.Subscribe<TrackChangedEvent>(OnTrackChanged);
            publicTransport.ApplicationEventBus.Subscribe<ShuffleChangedEvent>(OnShuffleChanged);
            publicTransport.ApplicationEventBus.Subscribe<PlayingStateChangedEvent>(OnPlayingStateChanged);
            publicTransport.ApplicationEventBus.Subscribe<TrackProgressEvent>(OnTrackProgress);
            volumeValue = configurationManager.GetValue("PlayerService.Volume", 1f, KnownConfigSections.Hidden);
            volumeValue.ValueChanged += VolumeValueOnValueChanged;
            this.playlistService = playlistService;
            this.publicTransport = publicTransport;
            this.dispatcher = dispatcher;
            seekTimer = new Timer(OnSeekTimer);
            Initialize();
        }

        private async void Initialize()
        {
            state = (await publicTransport.CommandBus.PublishWaitAsync(new GetStateCommand())).State;
            var file = (await publicTransport.CommandBus.PublishWaitAsync(new GetPlayingTrackCommand())).Track;
            if (null == file) return;
            ChangeTrack(playlistService.Files.Find(x => x.Filename == file.Name));
            SongLength = file.Length.TotalMilliseconds;
            SetTrackProgress(file.CurrentPositionMillisecond);
        }

        private void VolumeValueOnValueChanged(object sender, ValueChangedEventArgs<float> valueChangedEventArgs)
        {
            RaisePropertyChanged(() => Volume);
        }

        private void OnSeekTimer(object s)
        {
            publicTransport.CommandBus.Publish(new SeekCommand(currentPosition));
            seekTimer.Change(Timeout.Infinite, Timeout.Infinite);
            hasSought = false;
        }

        /// <summary>
        /// Called when [track progress].
        /// </summary>
        /// <param name="trackProgressEvent">The track progress event.</param>
        private void OnTrackProgress(TrackProgressEvent trackProgressEvent)
        {
            dispatcher.BeginInvoke(new Action<TrackProgressEvent>(a => SetTrackProgress(a.Position)), trackProgressEvent);
        }

        private void SetTrackProgress(double milliseconds)
        {
            var pos = TimeSpan.FromMilliseconds(milliseconds);
            CurrentPositionString = GetTimeString(pos);
            if (hasSought) return;
            currentPosition = milliseconds; // Don't set CurrentPosition directly as it will seek as well
            RaisePropertyChanged(() => CurrentPosition);
        }

        private static readonly TimeSpan SingletonHour = TimeSpan.FromHours(1);
        private static readonly TimeSpan SingletonDay = TimeSpan.FromDays(1);
        private static readonly TimeSpan SingletonYear = TimeSpan.FromDays(365);

        /// <summary>
        /// Gets the time string.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <returns></returns>
        private static string GetTimeString(TimeSpan span)
        {
            if (span >= SingletonHour)
            {
                if (span >= SingletonDay)
                {
                    return span >= SingletonYear ? "More than I can count with my fingers" : span.ToString(@"dd\:hh\:mm\:ss");
                }
                return span.ToString(@"hh\:mm\:ss");
            }
            return span.ToString(@"mm\:ss");
        }

        private void OnPlayingStateChanged(PlayingStateChangedEvent playingStateChangedEvent)
        {
            dispatcher.BeginInvoke(new Action<PlayingStateChangedEvent>(p =>
            {
                state = p.NewState;
                RaisePropertyChanged(() => IsPlaying);
            }), playingStateChangedEvent);
        }

        private void OnShuffleChanged(ShuffleChangedEvent shuffleChangedEvent)
        {
            dispatcher.BeginInvoke(new Action(() => RaisePropertyChanged(() => Shuffle)));
        }

        private void OnTrackChanged(TrackChangedEvent trackChangedEvent)
        {
            dispatcher.BeginInvoke(new Action<TrackChangedEvent>(tce =>
                                                  {
                                                      ChangeTrack(tce.File);
                                                      SongLength = tce.SongLength.TotalMilliseconds;
                                                  }), trackChangedEvent);
        }

        private void ChangeTrack(StorableTaggedFile newTrack)
        {
            NowPlaying = newTrack;
        }

        private string currentPositionString;

        /// <summary>
        /// Gets or sets the current position string.
        /// </summary>
        /// <milliseconds>
        /// The current position string.
        /// </milliseconds>
        public string CurrentPositionString
        {
            get { return currentPositionString; }
            set
            {
                if (value == currentPositionString) return;
                currentPositionString = value;
                RaisePropertyChanged(() => currentPositionString);
            }
        }

        public float Volume
        {
            get { return volumeValue.Value; }
            set { volumeValue.Value = value; }
        }

        private double currentPosition;

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        /// <milliseconds>
        /// The current position.
        /// </milliseconds>
        public double CurrentPosition
        {
            get { return currentPosition; }
            set
            {
                if (Math.Abs(value - currentPosition) <= double.Epsilon) return;
                hasSought = true;
                seekTimer.Change(250, 250);
                currentPosition = value;
                RaisePropertyChanged(() => CurrentPosition);
            }
        }

        private double songLength;

        /// <summary>
        /// Gets or sets the length of the song.
        /// </summary>
        /// <milliseconds>
        /// The length of the song.
        /// </milliseconds>
        public double SongLength
        {
            get { return songLength; }
            set
            {
                if (Math.Abs(value - songLength) <= double.Epsilon) return;
                songLength = value;
                RaisePropertyChanged(() => SongLength);
            }
        }

        /// <summary>
        /// Gets or sets a milliseconds indicating whether this <see cref="PlayerControlsViewModel" /> is shuffle.
        /// </summary>
        /// <milliseconds>
        ///   <c>true</c> if shuffle; otherwise, <c>false</c>.
        /// </milliseconds>
        public bool Shuffle
        {
            get { return playlistService.Shuffle; }
            set { publicTransport.CommandBus.Publish(new SetShuffleCommand(value)); }
        }

        /// <summary>
        /// Gets a milliseconds indicating whether this instance is playing.
        /// </summary>
        /// <milliseconds>
        /// <c>true</c> if this instance is playing; otherwise, <c>false</c>.
        /// </milliseconds>
        public bool IsPlaying
        {
            get { return state == PlayingState.Playing; }
        }

        private StorableTaggedFile nowPlaying;
        public StorableTaggedFile NowPlaying
        {
            get { return nowPlaying; }
            set
            {
                if (value == nowPlaying) return;
                nowPlaying = value;
                RaisePropertyChanged(() => NowPlaying);
            }
        }

        /// <summary>
        /// Gets the play pause command.
        /// </summary>
        /// <milliseconds>
        /// The play pause command.
        /// </milliseconds>
        public ICommand PlayPauseCommand
        {
            get { return playPauseCommand ?? (playPauseCommand = new DelegateCommand(OnPlayPause)); }
        }

        /// <summary>
        /// Gets the stop command.
        /// </summary>
        /// <milliseconds>
        /// The stop command.
        /// </milliseconds>
        public ICommand StopCommand
        {
            get { return stopCommand ?? (stopCommand = new DelegateCommand(OnStop)); }
        }

        /// <summary>
        /// Gets the previous command.
        /// </summary>
        /// <milliseconds>
        /// The previous command.
        /// </milliseconds>
        public ICommand PreviousCommand
        {
            get { return previousCommand ?? (previousCommand = new DelegateCommand(OnPrevious)); }
        }

        /// <summary>
        /// Gets the next command.
        /// </summary>
        /// <milliseconds>
        /// The next command.
        /// </milliseconds>
        public ICommand NextCommand
        {
            get { return nextCommand ?? (nextCommand = new DelegateCommand(OnNext)); }
        }

        private void OnNext()
        {
            publicTransport.CommandBus.Publish(new PlayNextCommand());
        }

        private void OnPrevious()
        {
            publicTransport.CommandBus.Publish(new PlayPreviousCommand());
        }

        private void OnStop()
        {
            publicTransport.CommandBus.Publish(new StopCommand());
        }

        private void OnPlayPause()
        {
            publicTransport.CommandBus.Publish(new PlayPauseCommand());
        }
    }
}

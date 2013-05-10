using System;
using System.Threading;
using System.Windows.Input;
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
        private readonly IPlayerService playerService;
        private readonly IPlaylistService playlistService;
        private readonly IDispatcher dispatcher;
        private ICommand playPauseCommand;
        private ICommand stopCommand;
        private ICommand previousCommand;
        private ICommand nextCommand;
        private PlayingState state;
        private readonly Timer seekTimer;
        private bool hasSought;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerControlsViewModel" /> class.
        /// </summary>
        /// <param name="playerService">The player service.</param>
        /// <param name="playlistService">The playlist service.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="logger">The logger.</param>
        public PlayerControlsViewModel(IPlayerService playerService, IPlaylistService playlistService, IPublicTransport publicTransport, IDispatcher dispatcher, ILogger logger)
            : base(logger)
        {
            playerService.Guard("playerService");
            publicTransport.Guard("publicTransport");
            dispatcher.Guard("dispatcher");
            publicTransport.ApplicationEventBus.Subscribe<TrackChangedEvent>(OnTrackChanged);
            publicTransport.ApplicationEventBus.Subscribe<ShuffleChangedEvent>(OnShuffleChanged);
            publicTransport.ApplicationEventBus.Subscribe<PlayingStateChangedEvent>(OnPlayingStateChanged);
            publicTransport.ApplicationEventBus.Subscribe<TrackProgressEvent>(OnTrackProgress);
            this.playerService = playerService;
            this.playlistService = playlistService;
            this.dispatcher = dispatcher;
            state = playerService.State;
            seekTimer = new Timer(OnSeekTimer);
        }

        private void OnSeekTimer(object s)
        {
            playerService.Seek(currentPosition);
            seekTimer.Change(Timeout.Infinite, Timeout.Infinite);
            hasSought = false;
        }

        /// <summary>
        /// Called when [track progress].
        /// </summary>
        /// <param name="trackProgressEvent">The track progress event.</param>
        private void OnTrackProgress(TrackProgressEvent trackProgressEvent)
        {
            dispatcher.BeginInvoke(new Action<TrackProgressEvent>(a =>
            {
                var pos = TimeSpan.FromMilliseconds(a.Position);
                CurrentPositionString = GetTimeString(pos);
                if (hasSought) return;
                currentPosition = a.Position; // Don't set CurrentPosition directly as it will seek as well
                RaisePropertyChanged(() => CurrentPosition);
            }), trackProgressEvent);
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
        /// <value>
        /// The current position string.
        /// </value>
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

        private double currentPosition;

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        /// <value>
        /// The current position.
        /// </value>
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
        /// <value>
        /// The length of the song.
        /// </value>
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
        /// Gets or sets a value indicating whether this <see cref="PlayerControlsViewModel" /> is shuffle.
        /// </summary>
        /// <value>
        ///   <c>true</c> if shuffle; otherwise, <c>false</c>.
        /// </value>
        public bool Shuffle
        {
            get { return playlistService.Shuffle; }
            set { playlistService.Shuffle = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is playing.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is playing; otherwise, <c>false</c>.
        /// </value>
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
        /// <value>
        /// The play pause command.
        /// </value>
        public ICommand PlayPauseCommand
        {
            get { return playPauseCommand ?? (playPauseCommand = new DelegateCommand(OnPlayPause)); }
        }

        /// <summary>
        /// Gets the stop command.
        /// </summary>
        /// <value>
        /// The stop command.
        /// </value>
        public ICommand StopCommand
        {
            get { return stopCommand ?? (stopCommand = new DelegateCommand(OnStop)); }
        }

        /// <summary>
        /// Gets the previous command.
        /// </summary>
        /// <value>
        /// The previous command.
        /// </value>
        public ICommand PreviousCommand
        {
            get { return previousCommand ?? (previousCommand = new DelegateCommand(OnPrevious)); }
        }

        /// <summary>
        /// Gets the next command.
        /// </summary>
        /// <value>
        /// The next command.
        /// </value>
        public ICommand NextCommand
        {
            get { return nextCommand ?? (nextCommand = new DelegateCommand(OnNext)); }
        }

        private void OnNext()
        {
            playerService.Next();
        }

        private void OnPrevious()
        {
            playerService.Previous();
        }

        private void OnStop()
        {
            playerService.Stop();
        }

        private void OnPlayPause()
        {
            playerService.PlayPause();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;
using iLynx.Common.Configuration;
using iLynx.Common.WPF;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace LMaML.Playlist.ViewModels
{
    /// <summary>
    /// ISearchViewModel
    /// </summary>
    public interface ISearchView : IRequestClose
    {
        /// <summary>
        /// Occurs when [play file].
        /// </summary>
        event Action<StorableTaggedFile> PlayFile;
    }

    /// <summary>
    /// SearchViewModel
    /// </summary>
    public class SearchViewModel : NotificationBase, ISearchView
    {
        private readonly IPlaylistService playlistService;
        private readonly IDispatcher dispatcher;
        private readonly Timer timer;
        private readonly IConfigurableValue<bool> staysOpen;
        private bool isSearching;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchViewModel" /> class.
        /// </summary>
        /// <param name="playlistService">The playlist service.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        public SearchViewModel(IPlaylistService playlistService,
            IPublicTransport publicTransport,
            IDispatcher dispatcher,
            IConfigurationManager configurationManager)
        {
            playlistService.Guard("playlistService");
            publicTransport.Guard("publicTransport");
            this.playlistService = playlistService;
            this.dispatcher = dispatcher;
            publicTransport.ApplicationEventBus.Subscribe<PlaylistUpdatedEvent>(OnPlaylistChanged);
            timer = new Timer(OnTimerTick);
            staysOpen = configurationManager.GetValue("Stays Open on Double Click", false, "Search Dialog");
        }

        private void OnTimerTick(object state)
        {
            FinishSearch();
        }

        private string filterString;

        /// <summary>
        /// Gets or sets the filter string.
        /// </summary>
        /// <value>
        /// The filter string.
        /// </value>
        public string FilterString
        {
            get { return filterString; }
            set
            {
                if (value == filterString) return;
                filterString = value;
                RaisePropertyChanged(() => FilterString);
                isSearching = true;
                timer.Change(TimeSpan.FromMilliseconds(250), Timeout.InfiniteTimeSpan);
            }
        }

        private void ResetFiles()
        {
            dispatcher.Invoke(() =>
                                  {
                                      Files = ApplyFilter(playlistService.Files);
                                      SelectedItem = Files.FirstOrDefault();
                                  });
        }

        private void OnPlaylistChanged(PlaylistUpdatedEvent playlistUpdatedEvent)
        {
            ResetFiles();
        }

        private IEnumerable<StorableTaggedFile> ApplyFilter(IEnumerable<StorableTaggedFile> source)
        {
            if (string.IsNullOrEmpty(filterString))
                return source;
            var regEx = new Regex(filterString, RegexOptions.IgnoreCase);
            return source.Where(x =>
                                regEx.IsMatch(x.Artist.Name) ||
                                regEx.IsMatch(x.Title.Name));
        }

        private IEnumerable<StorableTaggedFile> files;
        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        public IEnumerable<StorableTaggedFile> Files
        {
            get { return files; }
            set
            {
                if (ReferenceEquals(files, value)) return;
                files = value;
                RaisePropertyChanged(() => Files);
            }
        }

        private ICommand itemDoubleClicked;
        private StorableTaggedFile selectedItem;

        /// <summary>
        /// Gets the item double clicked.
        /// </summary>
        /// <value>
        /// The item double clicked.
        /// </value>
        public ICommand ItemDoubleClicked
        {
            get { return itemDoubleClicked ?? (itemDoubleClicked = new DelegateCommand(OnItemDoubleClicked)); }
        }

        private ICommand okClicked;

        /// <summary>
        /// Gets the ok clicked.
        /// </summary>
        /// <value>
        /// The ok clicked.
        /// </value>
        public ICommand OkCommand
        {
            get { return okClicked ?? (okClicked = new DelegateCommand(OnOkCommand)); }
        }

        private ICommand cancelClicked;
        /// <summary>
        /// Gets the cancel clicked.
        /// </summary>
        /// <value>
        /// The cancel clicked.
        /// </value>
        public ICommand CancelCommand
        {
            get { return cancelClicked ?? (cancelClicked = new DelegateCommand(OnCancelClicked)); }
        }

        private void OnCancelClicked()
        {
            OnRequestClose();
        }

        private void FinishSearch()
        {
            if (!isSearching) return;
            isSearching = false;
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            ResetFiles();
        }

        private void OnOkCommand()
        {
            FinishSearch();
            OnPlayFile();
            OnRequestClose();
        }

        private void OnItemDoubleClicked()
        {
            OnPlayFile();
            if (staysOpen.Value) return;
            OnRequestClose();
        }

        private void OnRequestClose()
        {
            if (null == RequestClose) return;
            RequestClose(this);
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>
        /// The selected item.
        /// </value>
        public StorableTaggedFile SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (value == selectedItem) return;
                selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
            }
        }

        /// <summary>
        /// Called when [play file].
        /// </summary>
        protected virtual void OnPlayFile()
        {
            if (null == PlayFile) return;
            PlayFile(selectedItem);
        }


        /// <summary>
        /// Occurs when [play file].
        /// </summary>
        public event Action<StorableTaggedFile> PlayFile;

        #region Implementation of IClosableItem

        public event Action<IRequestClose> RequestClose;

        #endregion
    }
}

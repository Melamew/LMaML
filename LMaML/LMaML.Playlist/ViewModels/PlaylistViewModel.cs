using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Util;
using iLynx.Common;
using iLynx.Common.Configuration;
using iLynx.Common.Serialization;
using iLynx.Common.WPF;

namespace LMaML.Playlist.ViewModels
{
    /// <summary>
    /// PlaylistViewModel
    /// </summary>
    public class PlaylistViewModel : NotificationBase
    {
        private readonly IPlaylistService playlistService;
        private readonly IDispatcher dispatcher;
        private readonly IPlayerService playerService;
        private readonly IInfoBuilder<StorableTaggedFile> builder;
        private readonly IGlobalHotkeyService globalHotkeyService;
        private readonly IWindowManager windowManager;
        private readonly ISearchView searchView;
        private List<StorableTaggedFile> files = new List<StorableTaggedFile>();
        private readonly IConfigurableValue<HotkeyDescriptor> searchHotkey;
        private ICommand doubleClickCommand;
        private ICommand keyUpCommand;

        /// <summary>
        /// Gets the delete selected command.
        /// </summary>
        /// <value>
        /// The delete selected command.
        /// </value>
        public ICommand DeleteSelectedCommand
        {
            get { return keyUpCommand ?? (keyUpCommand = new DelegateCommand<ICollection<object>>(OnDeleteSelected)); }
        }

        /// <summary>
        /// Called when [delete selected].
        /// </summary>
        /// <param name="collection">The collection.</param>
        private void OnDeleteSelected(ICollection<object> collection)
        {
            var copy = collection.OfType<StorableTaggedFile>().ToArray();
            foreach (var file in copy)
                Files.Remove(file);
            playlistService.RemoveFiles(copy);
            RaisePropertyChanged(() => Files);
        }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        public List<StorableTaggedFile> Files
        {
            get { return files; }
            set
            {
                if (value == files) return;
                files = value;
                RaisePropertyChanged(() => Files);
            }
        }

        private ICommand dropCommand;

        /// <summary>
        /// Gets the drop command.
        /// </summary>
        /// <value>
        /// The drop command.
        /// </value>
        public ICommand DropCommand
        {
            get { return dropCommand ?? (dropCommand = new DelegateCommand<DragEventArgs>(OnDragDrop)); }
        }

        private void OnDragDrop(DragEventArgs dragEventArgs)
        {
            if (!dragEventArgs.Data.GetFormats().Contains("FileDrop")) return;
            var fileNames = dragEventArgs.Data.GetData("FileDrop") as string[];
            if (null == fileNames) return;
            AddFilesAsync(fileNames);
        }

        private async void AddFilesAsync(IEnumerable<string> fileNames)
        {
            foreach (var dir in fileNames)
            {
                if (Directory.Exists(dir))
                    AddFiles(await RecursiveAsyncFileScanner<StorableTaggedFile>.GetFilesRecursiveAsync(new DirectoryInfo(dir), builder));
                else if (File.Exists(dir))
                {
                    bool valid;
                    var result = new[] { builder.Build(new FileInfo(dir), out valid) };
                    if (!valid) continue;
                    AddFiles(result);
                }
            }
        }

        /// <summary>
        /// Adds the files.
        /// </summary>
        /// <param name="ffs">The FFS.</param>
        private void AddFiles(IEnumerable<StorableTaggedFile> ffs)
        {
            dispatcher.Invoke(() => Files.AddRange(ffs));
        }

        private StorableTaggedFile selectedFile;

        /// <summary>
        /// Gets or sets the selected file.
        /// </summary>
        /// <value>
        /// The selected file.
        /// </value>
        public StorableTaggedFile SelectedFile
        {
            get { return selectedFile; }
            set
            {
                if (value == selectedFile) return;
                selectedFile = value;
                RaisePropertyChanged(() => SelectedFile);
            }
        }

        /// <summary>
        /// Gets the double click command.
        /// </summary>
        /// <value>
        /// The double click command.
        /// </value>
        public ICommand DoubleClickCommand
        {
            get { return doubleClickCommand ?? (doubleClickCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand<StorableTaggedFile>(OnDoubleClick)); }
        }

        /// <summary>
        /// Called when [double click].
        /// </summary>
        /// <param name="taggedFile">The tagged file.</param>
        private void OnDoubleClick(StorableTaggedFile taggedFile)
        {
            if (null == taggedFile) return;
            playerService.Play(taggedFile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistViewModel" /> class.
        /// </summary>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="playlistService">The playlist service.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="playerService">The audio player service.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="globalHotkeyService">The global hotkey service.</param>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="searchView">The search view.</param>
        /// <param name="serializerService">The serializer service.</param>
        /// <param name="logger">The logger.</param>
        public PlaylistViewModel(IPublicTransport publicTransport,
            IPlaylistService playlistService,
            IDispatcher dispatcher,
            IPlayerService playerService,
            IInfoBuilder<StorableTaggedFile> builder,
            IConfigurationManager configurationManager,
            IGlobalHotkeyService globalHotkeyService,
            IWindowManager windowManager,
            ISearchView searchView,
            ISerializerService serializerService,
            ILogger logger)
            : base(logger)
        {
            publicTransport.Guard("publicTransport");
            playlistService.Guard("playlistService");
            dispatcher.Guard("dispatcher");
            playerService.Guard("playerService");
            builder.Guard("builder");
            configurationManager.Guard("configurationManager");
            globalHotkeyService.Guard("globalHotkeyService");
            windowManager.Guard("windowManager");
            searchView.Guard("searchView");
            serializerService.Guard("serializerService");
            this.playlistService = playlistService;
            this.dispatcher = dispatcher;
            this.playerService = playerService;
            this.builder = builder;
            this.globalHotkeyService = globalHotkeyService;
            this.windowManager = windowManager;
            this.searchView = searchView;
            publicTransport.ApplicationEventBus.Subscribe<PlaylistUpdatedEvent>(OnPlaylistUpdated);
            publicTransport.ApplicationEventBus.Subscribe<TrackChangedEvent>(OnTrackChanged);
            searchHotkey = configurationManager.GetValue("Search", new HotkeyDescriptor(ModifierKeys.Control | ModifierKeys.Alt, Key.J),
                                                         KnownConfigSections.GlobalHotkeys);
            searchHotkey.ValueChanged += SearchHotkeyOnValueChanged;
            globalHotkeyService.RegisterHotkey(searchHotkey.Value, OnSearch);
            searchView.PlayFile += SearchViewOnPlayFile;
            Files = new List<StorableTaggedFile>(playlistService.Files);
        }

        private void SearchViewOnPlayFile(StorableTaggedFile storableTaggedFile)
        {
            playerService.Play(storableTaggedFile);
        }

        private void OnSearch()
        {
            windowManager.OpenNew(searchView, "Search", 400, 800);
        }

        private void SearchHotkeyOnValueChanged(object sender,
                                                ValueChangedEventArgs<HotkeyDescriptor> valueChangedEventArgs)
        {
            globalHotkeyService.UnregisterHotkey(valueChangedEventArgs.OldValue, OnSearch);
            globalHotkeyService.RegisterHotkey(valueChangedEventArgs.NewValue, OnSearch);
        }

        private void OnTrackChanged(TrackChangedEvent trackChangedEvent)
        {
            dispatcher.BeginInvoke(new Action(() => { SelectedFile = trackChangedEvent.File; }));
        }

        private void OnPlaylistUpdated(PlaylistUpdatedEvent e)
        {
            dispatcher.BeginInvoke(new Action(() => { Files = new List<StorableTaggedFile>(playlistService.Files); }));
        }
    }
}

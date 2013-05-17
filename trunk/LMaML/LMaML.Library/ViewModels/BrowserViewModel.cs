using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Threading;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Util;
using iLynx.Common;
using iLynx.Common.WPF;

namespace LMaML.Library.ViewModels
{
    /// <summary>
    /// BrowserViewModel
    /// </summary>'
    public class BrowserViewModel : NotificationBase
    {
        private readonly List<Alias<string>> localizedMemberPaths = new List<Alias<string>>();
        private readonly IDirectoryScannerService<StorableTaggedFile> scannerService;
        private readonly IPlaylistService playlistService;
        private readonly IPlayerService playerService;
        private readonly IDispatcher dispatcher;
        private readonly IFilteringService filteringService;
        private ObservableCollection<Alias<string>> columnSelectorItems;
        private readonly DispatcherTimer searchTimer;

        private IQueryable<StorableTaggedFile> results;

        private string filterString;

        public string FilterString
        {
            get { return filterString; }
            set
            {
                if (value == filterString) return;
                filterString = value;
                RaisePropertyChanged(() => FilterString);
                searchTimer.Start();
            }
        }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public IQueryable<StorableTaggedFile> Results
        {
            get { return results; }
            set
            {
                if (Equals(value, results)) return;
                results = value;
                RaisePropertyChanged(() => Results);
            }
        }

        private DynamicColumnViewModel firstColumn;
        /// <summary>
        /// Gets or sets the first column.
        /// </summary>
        /// <value>
        /// The first column.
        /// </value>
        public DynamicColumnViewModel FirstColumn
        {
            get { return firstColumn; }
            set
            {
                if (value == firstColumn) return;
                firstColumn = value;
                RaisePropertyChanged(() => FirstColumn);
            }
        }

        private DynamicColumnViewModel secondColumn;

        /// <summary>
        /// Gets or sets the second column.
        /// </summary>
        /// <value>
        /// The second column.
        /// </value>
        public DynamicColumnViewModel SecondColumn
        {
            get { return secondColumn; }
            set
            {
                if (value == secondColumn) return;
                secondColumn = value;
                RaisePropertyChanged(() => SecondColumn);
            }
        }
        private DynamicColumnViewModel thirdColumn;

        /// <summary>
        /// Gets or sets the third column.
        /// </summary>
        /// <value>
        /// The third column.
        /// </value>
        public DynamicColumnViewModel ThirdColumn
        {
            get { return thirdColumn; }
            set
            {
                if (value == thirdColumn) return;
                thirdColumn = value;
                RaisePropertyChanged(() => ThirdColumn);
            }
        }

        private Alias<string> currentFirstColumn;
        /// <summary>
        /// Gets the current first column.
        /// </summary>
        /// <value>
        /// The current first column.
        /// </value>
        public Alias<string> CurrentFirstColumn
        {
            get { return currentFirstColumn; }
            set
            {
                if (value == currentFirstColumn) return;
                currentFirstColumn = value;
                RaisePropertyChanged(() => CurrentFirstColumn);
                InitFirstColumn();
                firstColumn.SelectFirst();
            }
        }

        private Alias<string> currentSecondColumn;
        /// <summary>
        /// Gets or sets the current second column.
        /// </summary>
        /// <value>
        /// The current second column.
        /// </value>
        public Alias<string> CurrentSecondColumn
        {
            get { return currentSecondColumn; }
            set
            {
                if (value == currentSecondColumn) return;
                currentSecondColumn = value;
                RaisePropertyChanged(() => CurrentSecondColumn);
                FirstColumnOnItemSelected(firstColumn.SelectedItem);
                secondColumn.SelectFirst();
            }
        }

        private Alias<string> currentThirdColumn;
        /// <summary>
        /// Gets or sets the current third column.
        /// </summary>
        /// <value>
        /// The current third column.
        /// </value>
        public Alias<string> CurrentThirdColumn
        {
            get { return currentThirdColumn; }
            set
            {
                if (value == currentThirdColumn) return;
                currentThirdColumn = value;
                RaisePropertyChanged(() => CurrentThirdColumn);
                SecondColumnOnItemSelected(secondColumn.SelectedItem);
                thirdColumn.SelectFirst();
            }
        }

        /// <summary>
        /// Gets the column selector items.
        /// </summary>
        /// <value>
        /// The column selector items.
        /// </value>
        public ObservableCollection<Alias<string>> ColumnSelectorItems
        {
            get { return columnSelectorItems; }
            private set
            {
                if (value == columnSelectorItems) return;
                columnSelectorItems = value;
                RaisePropertyChanged(() => ColumnSelectorItems);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserViewModel" /> class.
        /// </summary>
        /// <param name="scannerService">The scanner.</param>
        /// <param name="playlistService">The playlist service.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="filteringService">The filtering service.</param>
        /// <param name="logger">The logger</param>
        /// <param name="menuService">Blah</param>
        public BrowserViewModel(IDirectoryScannerService<StorableTaggedFile> scannerService,
                                IPlaylistService playlistService,
                                IPlayerService playerService,
                                IDispatcher dispatcher,
                                IFilteringService filteringService,
                                ILogger logger,
                                IMenuService menuService)
            : base(logger)
        {
            scannerService.Guard("scannerService");
            playlistService.Guard("playlistService");
            dispatcher.Guard("dispatcher");
            filteringService.Guard("filteringService");
            menuService.Guard("menuService");
            // TODO: Localize
            menuService.Register(new CallbackMenuItem(null, "Library", new CallbackMenuItem(OnAddFiles, "Add Files")));
            this.scannerService = scannerService;
            this.playlistService = playlistService;
            this.playerService = playerService;
            this.dispatcher = dispatcher;
            this.filteringService = filteringService;
            this.scannerService.ScanCompleted += ScannerServiceOnScanCompleted;
            this.scannerService.ScanProgress += ScannerServiceOnScanProgress;
            localizedMemberPaths = filteringService.FilterColumns.Select(x => new Alias<string>(x, x)).ToList(); // TODO: Localize
            searchTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            searchTimer.Tick += SearchTimerOnTick;
            FirstColumn = new DynamicColumnViewModel(logger);
            SecondColumn = new DynamicColumnViewModel(logger);
            ThirdColumn = new DynamicColumnViewModel(logger);
            InitViewModels();
            BuildColumns();
            InitFirstColumn();
        }

        private void SearchTimerOnTick(object sender, EventArgs eventArgs)
        {
            var filter = string.IsNullOrEmpty(filterString) ? null : new Regex(filterString, RegexOptions.IgnoreCase);
            firstColumn.SetFilter(filter);
            secondColumn.SetFilter(filter);
            thirdColumn.SetFilter(filter);
            searchTimer.Stop();
        }

        private void BuildColumns()
        {
            ColumnSelectorItems = new ObservableCollection<Alias<string>>(localizedMemberPaths);
            CurrentFirstColumn = ColumnSelectorItems.FirstOrDefault();
            CurrentSecondColumn = ColumnSelectorItems.FirstOrDefault(f => f != CurrentFirstColumn);
            CurrentThirdColumn = ColumnSelectorItems.FirstOrDefault(f => f != CurrentFirstColumn && f != CurrentSecondColumn);
        }

        private void InitViewModels()
        {
            FirstColumn.ItemSelected += FirstColumnOnItemSelected;
            FirstColumn.ItemDoubleClicked += ColumnOnItemDoubleClicked;
            FirstColumn.ItemClicked += FirstColumnOnItemClicked;
            SecondColumn.ItemSelected += SecondColumnOnItemSelected;
            SecondColumn.ItemDoubleClicked += ColumnOnItemDoubleClicked;
            SecondColumn.ItemClicked += SecondColumnOnItemClicked;
            ThirdColumn.ItemSelected += ThirdColumnOnItemSelected;
            ThirdColumn.ItemDoubleClicked += ColumnOnItemDoubleClicked;
            var mem = RuntimeHelper.GetMemberName(() => FilterAll.Singleton.Name);
            FirstColumn.DisplayMember = mem;
            SecondColumn.DisplayMember = mem;
            ThirdColumn.DisplayMember = mem;
        }

        private async void InitFirstColumn()
        {
            if (null == CurrentFirstColumn) return;
            firstColumn.SetItems(await filteringService.GetFullColumnAsync(CurrentFirstColumn.Original));
        }

        private async void FirstColumnOnItemSelected(TagReference tagReference)
        {
            if (null == CurrentSecondColumn) return;
            if (null == firstColumn.SelectedItem) return;
            secondColumn.SetItems(await filteringService.GetColumnAsync(CurrentSecondColumn.Original, new ColumnSetup(CurrentFirstColumn.Original, firstColumn.SelectedItem.Id)));
        }

        private void SecondColumnOnItemClicked()
        {
            ThirdColumn.SelectFirst();
        }

        private async void SecondColumnOnItemSelected(TagReference tagReference)
        {
            if (null == CurrentThirdColumn) return;
            if (null == secondColumn.SelectedItem) return;
            if (null == firstColumn.SelectedItem) return;
            thirdColumn.SetItems(
                await
                filteringService.GetColumnAsync(CurrentThirdColumn.Original,
                                                new ColumnSetup(CurrentFirstColumn.Original, firstColumn.SelectedItem.Id),
                                                new ColumnSetup(CurrentSecondColumn.Original, secondColumn.SelectedItem.Id)));
        }

        private void ColumnOnItemDoubleClicked(TagReference tagReference)
        {
            if (null == results) return;
            playlistService.Clear();
            playlistService.AddFiles(results);
            playerService.Next();
        }

        private void ThirdColumnOnItemSelected(TagReference tagReference)
        {
            UpdateResults();
        }

        /// <summary>
        /// Firsts the column on item clicked.
        /// </summary>
        private void FirstColumnOnItemClicked()
        {
            SecondColumn.SelectFirst();
        }

        private async void UpdateResults()
        {
            var setups = new List<IColumnSetup>();
            if (null != firstColumn.SelectedItem)
                setups.Add(new ColumnSetup(currentFirstColumn.Original, firstColumn.SelectedItem.Id));
            if (null != secondColumn.SelectedItem)
                setups.Add(new ColumnSetup(currentSecondColumn.Original, secondColumn.SelectedItem.Id));
            if (null != thirdColumn.SelectedItem)
                setups.Add(new ColumnSetup(currentThirdColumn.Original, thirdColumn.SelectedItem.Id));
            if (setups.Count <= 0) return; // You never know...
            var items = await filteringService.GetFilesAsync(setups.ToArray());
            Results = items;
        }

        private bool isIndeterminate;

        public bool IsIndeterminate
        {
            get { return isIndeterminate; }
            set
            {
                if (value == isIndeterminate) return;
                isIndeterminate = value;
                RaisePropertyChanged(() => IsIndeterminate);
            }
        }

        private double scanPercent;

        /// <summary>
        /// Gets or sets the scan percent.
        /// </summary>
        /// <value>
        /// The scan percent.
        /// </value>
        public double ScanPercent
        {
            get { return scanPercent; }
            set
            {
#pragma warning disable 665 // Intentional...
                if (IsIndeterminate = (value < 0)) return;
#pragma warning restore 665
                if (Math.Abs(value - scanPercent) <= double.Epsilon) return;
                scanPercent = value;
                RaisePropertyChanged(() => ScanPercent);
            }
        }

        private void ScannerServiceOnScanProgress(double d)
        {
            dispatcher.Invoke(p => { ScanPercent = p; }, d);
        }

        private void ScannerServiceOnScanCompleted(object sender, EventArgs eventArgs)
        {
            dispatcher.Invoke(InitFirstColumn);
        }

        private void OnAddFiles()
        {
            var dialog = new FolderBrowserDialog
                             {
                                 ShowNewFolderButton = true,
                                 // ReSharper disable LocalizableElement
                                 Description = "Media Folder Selector",
                                 // ReSharper restore LocalizableElement
                             };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            scannerService.Scan(dialog.SelectedPath);
        }
    }
}

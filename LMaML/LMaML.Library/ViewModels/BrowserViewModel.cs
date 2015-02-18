using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Commands;
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
    public class BrowserViewModel : LoadScreenViewModelBase
    {
        private readonly IPublicTransport publicTransport;
        private readonly List<Alias<string>> localizedMemberPaths = new List<Alias<string>>();
        private readonly IDirectoryScannerService<StorableTaggedFile> scannerService;
        private readonly IDispatcher dispatcher;
        private readonly IFilteringService filteringService;
        private ICommand doubleClickCommand;
        private ObservableCollection<Alias<string>> columnSelectorItems;
        private readonly DispatcherTimer searchTimer;
        private StorableTaggedFile selectedResult;

        public StorableTaggedFile SelectedResult
        {
            get { return selectedResult; }
            set
            {
                if (value == selectedResult) return;
                selectedResult = value;
                OnPropertyChanged();
            }
        }

        public ICommand DoubleClickCommand
        {
            get { return doubleClickCommand ?? (doubleClickCommand = new DelegateCommand<StorableTaggedFile>(OnFileDoubleClicked)); }
        }

        private void OnFileDoubleClicked(StorableTaggedFile obj)
        {
            if (null == obj) return;
            publicTransport.CommandBus.Publish(new AddFilesCommand(new[] { obj }));
            publicTransport.CommandBus.Publish(new PlayFileCommand(obj));
        }

        private StorableTaggedFile[] results;

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
        public IEnumerable<StorableTaggedFile> Results
        {
            get { return results; }
            set
            {
                if (Equals(value, results)) return;
                WorkerMessage = "Loading...";
                IsLoading = true;
#pragma warning disable 4014
                Task.Run(() =>
#pragma warning restore 4014
                {
                    var res = value.ToArray();
                    dispatcher.Invoke(() =>
                    {
                        results = res;
                        IsLoading = false;
                        RaisePropertyChanged(() => Results);
                        RaisePropertyChanged(() => HitCount);
                    });
                });
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
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="filteringService">The filtering service.</param>
        /// <param name="menuService">Blah</param>
        /// <param name="referenceAdapters">The reference adapters.</param>
        /// <param name="publicTransport"></param>
        public BrowserViewModel(IDirectoryScannerService<StorableTaggedFile> scannerService,
                                IDispatcher dispatcher,
                                IFilteringService filteringService,
                                IMenuService menuService,
                                IReferenceAdapters referenceAdapters,
                                IPublicTransport publicTransport)
        {
            this.publicTransport = Guard.IsNull(() => publicTransport);
            scannerService.Guard("scannerService");
            dispatcher.Guard("dispatcher");
            filteringService.Guard("filteringService");
            menuService.Guard("menuService");
            referenceAdapters.Guard("referenceAdapters");
            // TODO: Localize
            menuService.Register(new CallbackMenuItem(null, "Library", new CallbackMenuItem(OnAddFiles, "Add Files")));
            this.scannerService = Guard.IsNull(() => scannerService);
            this.dispatcher = Guard.IsNull(() => dispatcher);
            this.filteringService = Guard.IsNull(() => filteringService);
            this.scannerService.ScanCompleted += ScannerServiceOnScanCompleted;
            this.scannerService.ScanProgress += ScannerServiceOnScanProgress;
            localizedMemberPaths = filteringService.FilterColumns.Select(x => new Alias<string>(x, x)).ToList(); // TODO: Localize
            searchTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            searchTimer.Tick += SearchTimerOnTick;
            FirstColumn = new DynamicColumnViewModel(dispatcher);
            SecondColumn = new DynamicColumnViewModel(dispatcher);
            ThirdColumn = new DynamicColumnViewModel(dispatcher);
            InitViewModels();
            BuildColumns();
            InitFirstColumn();
        }

        private void OnAddItems()
        {
            publicTransport.CommandBus.Publish(new AddFilesCommand(results));
        }

        private void OnPlayItems()
        {
            publicTransport.CommandBus.PublishWait(new SetPlaylistCommand(results));
            publicTransport.CommandBus.Publish(new PlayNextCommand());
        }

        private void SearchTimerOnTick(object sender, EventArgs eventArgs)
        {
            Regex filter;
            try
            {
                filter = string.IsNullOrEmpty(filterString)
                    ? null
                    : new Regex(filterString, RegexOptions.IgnoreCase);
            }
            catch (ArgumentException)
            {
                return;
            }
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
            FirstColumn.PlayItems += OnPlayItems;
            FirstColumn.AddItems += OnAddItems;

            SecondColumn.ItemSelected += SecondColumnOnItemSelected;
            SecondColumn.ItemDoubleClicked += ColumnOnItemDoubleClicked;
            SecondColumn.PlayItems += OnPlayItems;
            SecondColumn.AddItems += OnAddItems;

            ThirdColumn.ItemSelected += ThirdColumnOnItemSelected;
            ThirdColumn.ItemDoubleClicked += ColumnOnItemDoubleClicked;
            ThirdColumn.PlayItems += OnPlayItems;
            ThirdColumn.AddItems += OnAddItems;
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
            secondColumn.Clear();
            thirdColumn.Clear();
            secondColumn.SetItems(await filteringService.GetColumnAsync(CurrentSecondColumn.Original, new ColumnSetup(CurrentFirstColumn.Original, firstColumn.SelectedItem.Id)));
        }

        private async void SecondColumnOnItemSelected(TagReference tagReference)
        {
            if (null == CurrentThirdColumn) return;
            if (null == secondColumn.SelectedItem) return;
            if (null == firstColumn.SelectedItem) return;
            thirdColumn.Clear();
            thirdColumn.SetItems(
                await
                filteringService.GetColumnAsync(CurrentThirdColumn.Original,
                                                new ColumnSetup(CurrentFirstColumn.Original, firstColumn.SelectedItem.Id),
                                                new ColumnSetup(CurrentSecondColumn.Original, secondColumn.SelectedItem.Id)));
        }

        public int HitCount
        {
            get { return null == results ? 0 : results.Length; }
        }

        private void ThirdColumnOnItemSelected(TagReference tagReference)
        {
            UpdateResults();
        }

        private void ColumnOnItemDoubleClicked(TagReference tagReference)
        {
            if (null == results) return;
            OnPlayItems();
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

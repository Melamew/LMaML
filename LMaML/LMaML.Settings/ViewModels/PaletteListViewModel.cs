using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using iLynx.Common;
using iLynx.Common.Pixels;
using iLynx.Configuration;
using LMaML.Infrastructure;

namespace LMaML.Settings.ViewModels
{
    public class PaletteListViewModel : SettingsValueViewModelBase<IPalette<double>>
    {
        private readonly ObservableCollection<PaletteEntryViewModel> entries;

        public PaletteListViewModel(IConfigurableValue value)
            : base(value)
        {
            entries = new ObservableCollection<PaletteEntryViewModel>(Value.GetMap().Select(x =>
                                                                                            {
                                                                                                var result =
                                                                                                    new PaletteEntryViewModel(x.Item1,
                                                                                                        x.Item2);
                                                                                                result.Commit += ResultOnCommit;
                                                                                                return result;
                                                                                            }));
        }

        private void ResultOnCommit(PaletteEntryViewModel paletteEntryViewModel)
        {
            if (!paletteEntryViewModel.IsNew)
                Value.RemoveValue(paletteEntryViewModel.OriginalValue);
            Value.MapValue(paletteEntryViewModel.Value, paletteEntryViewModel.Colour);
        }

        public ObservableCollection<PaletteEntryViewModel> Entries
        {
            get { return entries; }
        }
    }

    public class PaletteEntryViewModel : NotificationBase
    {
        private double value;
        private Color colour;
        private bool isColourPickerOpen;
        private readonly double originalValue;
        private bool isNew = false;

        public PaletteEntryViewModel()
        {
            isNew = true;
        }

        public PaletteEntryViewModel(double value, Color colour)
        {
            originalValue = value;
            this.value = value;
            this.colour = colour;
        }

        public event Action<PaletteEntryViewModel> Commit;

        protected virtual void OnCommit()
        {
            var handler = Commit;
            if (null == handler) return;
            handler(this);
            isNew = false;
        }

        public bool IsColourPickerOpen
        {
            get { return isColourPickerOpen; }
            set
            {
                if (value == isColourPickerOpen) return;
                isColourPickerOpen = value;
                OnPropertyChanged();
                if (!isColourPickerOpen)
                    OnCommit();
            }
        }

        public double Value
        {
            get
            {
                return value;
            }
            set
            {
                if (Math.Abs(this.value - value) < double.Epsilon) return;
                this.value = value;
                OnPropertyChanged();
                OnCommit();
            }
        }

        public Color Colour
        {
            get
            {
                return colour;
            }
            set
            {
                if (value == colour) return;
                colour = value;
                OnPropertyChanged();
            }
        }

        public double OriginalValue
        {
            get { return originalValue; }
        }

        public bool IsNew
        {
            get { return isNew; }
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using iLynx.Common;
using iLynx.Common.Pixels;
using iLynx.Common.WPF;
using iLynx.Configuration;
using LMaML.Infrastructure;

namespace LMaML.Settings.ViewModels
{
    public class PaletteListViewModel : SettingsValueViewModelBase<IPalette<double>>
    {
        private readonly ObservableCollection<PaletteEntryViewModel> entries;
        private ICommand addCommand;

        public PaletteListViewModel(IConfigurableValue value)
            : base(value)
        {
            entries = new ObservableCollection<PaletteEntryViewModel>(Value.GetMap().Select(x =>
                                                                                            {
                                                                                                var result =
                                                                                                    new PaletteEntryViewModel(x.Item1,
                                                                                                        x.Item2);
                                                                                                Subscribe(result);
                                                                                                return result;
                                                                                            }));
        }

        private void ResultOnDelete(PaletteEntryViewModel paletteEntryViewModel)
        {
            if (!paletteEntryViewModel.IsNew)
                Value.RemoveValue(paletteEntryViewModel.Value);
            entries.Remove(paletteEntryViewModel);
            Unsubscribe(paletteEntryViewModel);
        }

        private void ResultOnColourChanged(PaletteEntryViewModel vm)
        {
            if (vm.IsNew && Value.Contains(vm.Value))
                return; // Don't want to overwrite any existing values.
            Value.MapValue(vm.Value, vm.Colour); // Doesn't matter if it's new or not, MapValue should overwrite the existing colour.
        }

        private void ResultOnValueChanged(PaletteEntryViewModel vm)
        {
            if (vm.IsNew || Value.Contains(vm.Value))
                return;
            if (vm.IsNew)
                Value.MapValue(vm.Value, vm.Colour);
            else
                Value.RemapValue(vm.OriginalValue, vm.Value);
        }

        public ObservableCollection<PaletteEntryViewModel> Entries
        {
            get { return entries; }
        }

        public ICommand AddCommand
        {
            get { return addCommand ?? (addCommand = new DelegateCommand(OnAdd)); }
        }

        private void OnAdd()
        {
            var value = Value.MaxValue + 1d;
            var vm = new PaletteEntryViewModel(value, Colors.White);
            Value.MapValue(vm.Value, vm.Colour);
            Subscribe(vm);
            entries.Add(vm);
        }

        private void Subscribe(PaletteEntryViewModel vm)
        {
            vm.ValueChanged += ResultOnValueChanged;
            vm.ColourChanged += ResultOnColourChanged;
            vm.Delete += ResultOnDelete;
        }

        private void Unsubscribe(PaletteEntryViewModel vm)
        {
            vm.ValueChanged -= ResultOnValueChanged;
            vm.ColourChanged -= ResultOnColourChanged;
            vm.Delete -= ResultOnDelete;
        }
    }

    public class PaletteEntryViewModel : NotificationBase
    {
        private double value;
        private Color colour;
        private bool isColourPickerOpen;
        private double originalValue;
        private bool isNew;
        private ICommand deleteCommand;

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

        public event Action<PaletteEntryViewModel> ValueChanged;
        public event Action<PaletteEntryViewModel> ColourChanged;
        public event Action<PaletteEntryViewModel> Delete;

        public ICommand DeleteCommand
        {
            get { return deleteCommand ?? (deleteCommand = new DelegateCommand(DoDelete)); }
        }

        private void DoDelete()
        {
            OnDelete();
        }

        protected virtual void OnDelete()
        {
            Invoke(Delete);
        }

        protected virtual void OnColourChanged()
        {
            Invoke(ColourChanged);
        }

        private void Invoke(Action<PaletteEntryViewModel> handler)
        {
            if (null == handler) return;
            handler(this);
            isNew = false;
        }

        protected virtual void OnValueChanged()
        {
            Invoke(ValueChanged);
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
                    OnColourChanged();
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
                originalValue = this.value;
                this.value = value;
                OnPropertyChanged();
                OnValueChanged();
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

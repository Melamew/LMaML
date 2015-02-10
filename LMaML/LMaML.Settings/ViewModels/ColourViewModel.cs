using System.Windows.Media;
using iLynx.Configuration;
using LMaML.Infrastructure;

namespace LMaML.Settings.ViewModels
{
    public class ColourViewModel : SettingsValueViewModelBase<Color>
    {
        public ColourViewModel(IConfigurableValue value)
            : base(value, false)
        {
        }

        private bool isOpen;

        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                if (value == isOpen) return;
                isOpen = value;
                OnPropertyChanged();
                if (!isOpen)
                    Save();
            }
        }
    }
}

using iLynx.Configuration;
using LMaML.Infrastructure;

namespace LMaML.Settings.ViewModels
{
    public class CheckBoxViewModel : SettingsValueViewModelBase<bool>
    {
        public CheckBoxViewModel(IConfigurableValue value) : base(value)
        {
        }
    }
}

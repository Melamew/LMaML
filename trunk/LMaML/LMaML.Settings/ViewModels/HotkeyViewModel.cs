using System.Windows.Input;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;
using iLynx.Common.Configuration;
using iLynx.Common.WPF;

namespace LMaML.Settings.ViewModels
{
    /// <summary>
    /// HotkeyViewModel
    /// </summary>
    public class HotkeyViewModel : NotificationBase
    {
        private readonly IConfigurableValue<HotkeyDescriptor> descriptorValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyViewModel" /> class.
        /// </summary>
        /// <param name="descriptorValue">The descriptor value.</param>
        public HotkeyViewModel(IConfigurableValue<HotkeyDescriptor> descriptorValue)
        {
            this.descriptorValue = descriptorValue;
            Name = descriptorValue.Key;
        }

        public string Name { get; private set; }

        public string Value
        {
            get { return string.Format("<{0}><{1}>", descriptorValue.Value.Modifiers, descriptorValue.Value.Key); }
        }

        private ICommand keyDownCommand;
        public ICommand KeyDownCommand
        {
            get { return keyDownCommand ?? (keyDownCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand<KeyEventArgs>(OnKeyDown)); }
        }

        private void OnKeyDown(KeyEventArgs e)
        {
            descriptorValue.Value = new HotkeyDescriptor(e.KeyboardDevice.Modifiers, e.Key);
            descriptorValue.Store();
            RaisePropertyChanged(() => Value);
        }
    }
}
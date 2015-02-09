using System.Collections.Generic;
using System.Linq;
using iLynx.Common;
using iLynx.Configuration;
using LMaML.Infrastructure;

namespace LMaML.Settings.ViewModels
{
    public class ComboBoxViewModel : SettingsValueViewModelBase
    {
        private readonly IEnumerable<object> values;

        public ComboBoxViewModel(IEnumerable<object> values, IConfigurableValue value) : base(value)
        {
            this.values = values ?? Enumerable.Empty<object>();
        }

        public ComboBoxViewModel(IConfigurableValue value, params object[] values) : base(value)
        {
            this.values = Guard.IsNull(() => values);
        }

        public IEnumerable<object> Values
        {
            get { return values; }
        } 
    }
}

using System;
using iLynx.Configuration;

namespace LMaML.Infrastructure.Services
{
    public interface IValueEditorViewFactory
    {
        void RegisterBuilder(Type valueType, Func<IConfigurableValue, SettingsValueViewModelBase> callback);
        object CreateFor(IConfigurableValue value);
    }
}
using iLynx.Configuration;

namespace LMaML.Infrastructure.Services
{
    public interface IValueEditorViewFactory
    {
        object CreateFor(IConfigurableValue value);
    }
}
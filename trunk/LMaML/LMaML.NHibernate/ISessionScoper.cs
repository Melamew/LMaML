using NHibernate;

namespace LMaML.NHibernate
{
    /// <summary>
    /// ISessionScoper
    /// </summary>
    public interface ISessionScoper
    {
        IStatelessSession GetSession();
    }
}
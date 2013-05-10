using NHibernate;

namespace LMaML.NHibernate
{
    /// <summary>
    /// StaticSessionScoper
    /// </summary>
    public class SingletonSessionScoper : ISessionScoper
    {
        private readonly ISessionFactory factory;
        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonSessionScoper" /> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public SingletonSessionScoper(ISessionFactory factory)
        {
            this.factory = factory;
        }

        private static IStatelessSession session;
        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <returns></returns>
        public IStatelessSession GetSession()
        {
            return session ?? (session = factory.OpenStatelessSession());
        }
    }
}
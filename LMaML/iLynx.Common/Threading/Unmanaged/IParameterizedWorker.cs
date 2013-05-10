namespace iLynx.Common.Threading.Unmanaged
{
    /// <summary>
    /// IWorker
    /// </summary>
    /// <typeparam name="TParams">The type of the params.</typeparam>
    public interface IParameterizedWorker<in TParams> : IWorker
    {
        /// <summary>
        /// Executes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        void Execute(TParams args);
    }
}
namespace iLynx.Common.Threading.Unmanaged
{
    /// <summary>
    /// IParameterizedResultWorker
    /// </summary>
    /// <typeparam name="TParams">The type of the params.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IParameterizedResultWorker<in TParams, out TResult> : IResultWorker<TResult>, IParameterizedWorker<TParams>
    {
        
    }
}
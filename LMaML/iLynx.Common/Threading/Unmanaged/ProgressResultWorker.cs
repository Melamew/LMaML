namespace iLynx.Common.Threading.Unmanaged
{
    /// <summary>
    /// This class can be used to run an asynchronous operation while also providing progress updates to users
    /// </summary>
    /// <typeparam name="TArgs">The type of arguments this worker will use</typeparam>
    /// <typeparam name="TCompletedArgs">The type of results the worker will produce</typeparam>
    public abstract class ProgressResultWorker<TArgs, TCompletedArgs> : ThreadedResultWorker<TArgs, TCompletedArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressResultWorker{TArgs,TCompletedArgs}" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected ProgressResultWorker(ILogger logger) : base(logger)
        {
        }

        /// <summary>
        /// This event is fired when the underlying worker has progress in the form percent to report
        /// </summary>
        public event GenericEventHandler<ProgressResultWorker<TArgs, TCompletedArgs>, double> Progress;

        /// <summary>
        /// This event is fired when the underlying worker has progress in the form of a message to report
        /// </summary>
        public event GenericEventHandler<ProgressResultWorker<TArgs, TCompletedArgs>, string> Status;

        /// <summary>
        /// Used to fire the <see cref="Progress"/> event
        /// </summary>
        /// <param name="progress"></param>
        protected virtual void OnProgress(double progress)
        {
            if (Progress != null)
                Progress(this, progress);
        }

        /// <summary>
        /// Used to fire the <see cref="Status"/> event
        /// </summary>
        /// <param name="text"></param>
        protected virtual void OnProgress(string text)
        {
            if (Status != null)
                Status(this, text);
        }

        /// <summary>
        /// This method is a convenience method for calling both <see cref="OnProgress(double)"/> and <see cref="OnProgress(string)"/>
        /// <para/>
        /// The two methods are called in the order they are listed above.
        /// </summary>
        /// <param name="progress">The progress to report</param>
        /// <param name="text">The message to report</param>
        protected virtual void OnProgress(string text, double progress)
        {
            OnProgress(progress);
            OnProgress(text);
        }
    }
}
using iLynx.Common;

namespace LMaML.Infrastructure
{
    public abstract class LoadScreenViewModelBase : NotificationBase
    {
        private string workerMessage;
        private bool isLoading;

        /// <summary>
        /// Gets or Sets a value indicating whether or not a background worker is currently running for this viewmodel
        /// </summary>
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (value == isLoading) return;
                isLoading = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or Sets the message that should be displayed in the loadscreen.
        /// </summary>
        public string WorkerMessage
        {
            get { return workerMessage; }
            set
            {
                if (value == workerMessage) return;
                workerMessage = value;
                OnPropertyChanged();
            }
        }
    }
}

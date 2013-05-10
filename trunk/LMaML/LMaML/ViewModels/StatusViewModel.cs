using System;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;
using iLynx.Common.WPF;

namespace LMaML.ViewModels
{
    /// <summary>
    /// StatusViewModel
    /// </summary>
    public class StatusViewModel : NotificationBase
    {
        private readonly IDispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusViewModel" /> class.
        /// </summary>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        public StatusViewModel(IPublicTransport publicTransport, IDispatcher dispatcher)
        {
            publicTransport.Guard("publicTransport");
            dispatcher.Guard("dispatcher");
            this.dispatcher = dispatcher;
            publicTransport.ApplicationEventBus.Subscribe<ProgressEvent>(OnProgress);
            publicTransport.ApplicationEventBus.Subscribe<WorkStartedEvent>(OnWorkStarted);
            publicTransport.ApplicationEventBus.Subscribe<WorkCompletedEvent>(OnWorkCompleted);
        }

        private void OnWorkCompleted(WorkCompletedEvent workCompletedEvent)
        {
            // TODO: Implement multiple
        }

        private void OnWorkStarted(WorkStartedEvent workStartedEvent)
        {
            // TODO: Implement multiple
        }

        /// <summary>
        /// Called when [progress].
        /// </summary>
        /// <param name="progressEvent">The progress event.</param>
        private void OnProgress(ProgressEvent progressEvent)
        {
            // TODO: Implement multiple
            dispatcher.Invoke(x =>
                                  {
                                      CurrentProgress = x.Progress;
                                      CurrentStatus = x.ProgressString;
                                  }, progressEvent);
        }

        private string currentStatus;

        /// <summary>
        /// Gets the current status.
        /// </summary>
        /// <value>
        /// The current status.
        /// </value>
        public string CurrentStatus
        {
            get { return currentStatus; }
            private set
            {
                if (value == currentStatus) return;
                currentStatus = value;
                RaisePropertyChanged(() => CurrentStatus);
            }
        }

        private double currentProgress;
        /// <summary>
        /// Gets the current progress.
        /// </summary>
        /// <value>
        /// The current progress.
        /// </value>
        public double CurrentProgress
        {
            get { return currentProgress; }
            private set
            {
                if (Math.Abs(currentProgress - value) <= double.Epsilon) return;
                currentProgress = value;
                RaisePropertyChanged(() => CurrentProgress);
            }
        }
    }
}

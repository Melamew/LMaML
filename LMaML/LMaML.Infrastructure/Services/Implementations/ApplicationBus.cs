using System;
using iLynx.PubSub;
using iLynx.Threading;
using LMaML.Infrastructure.Services.Interfaces;

namespace LMaML.Infrastructure.Services.Implementations
{
    public class ApplicationBus : QueuedBus<IBusMessage>
    {
        public ApplicationBus(IThreadManager threadManager)
            : base(threadManager)
        {
        }

        protected override void Publish(Type messageType, dynamic message)
        { 
            base.Publish(messageType, (IBusMessage)message);
            message.Handled = true;
        }
    }
}

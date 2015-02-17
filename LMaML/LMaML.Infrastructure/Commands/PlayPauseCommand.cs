using System;
using System.Threading;
using System.Threading.Tasks;
using iLynx.PubSub;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;

namespace LMaML.Infrastructure.Commands
{
    public abstract class BusMessage : IBusMessage
    {
        private bool isHandled;

        public bool IsHandled
        {
            get { return isHandled; }
            set
            {
                isHandled = value;
                if (value)
                    OnHandled();
            }
        }

        public event Action Handled;

        protected virtual void OnHandled()
        {
            var handler = Handled;
            if (null == handler) return;
            handler();
        }

        public void Wait(TimeSpan? timeout = null)
        {
            var start = DateTime.UtcNow;
            while (!IsHandled)
            {
                Thread.CurrentThread.Join(5);
                if (null != timeout && (DateTime.UtcNow - start) >= timeout)
                    throw new TimeoutException();
            }
        }
    }

    public abstract class BusMessage<TResult> : BusMessage, IBusMessage<TResult>
    {
        public TResult Result { get; set; }
    }

    public abstract class ApplicationEvent : BusMessage, IApplicationEvent { }

    public static class BusExtensions
    {
        public static async Task<TMessage> PublishWaitAsync<TMessage>(this IBus<IBusMessage> bus, TMessage message,
            TimeSpan? timeout = null)
            where TMessage : IBusMessage
        {
            return await Task.Run(() => bus.PublishWait(message, timeout));
        }

        public static TMessage PublishWait<TMessage>(this IBus<IBusMessage> bus, TMessage message, TimeSpan? timeout = null)
            where TMessage : IBusMessage
        {
            bus.Publish(message);
            message.Wait(timeout);
            return message;
        }

        public static TResult GetResult<TResult>(this IBus<IBusMessage> bus, IBusMessage<TResult> message, TimeSpan? timeout = null)
        {
            bus.Publish(message);
            message.Wait(timeout);
            return message.Result;
        }

        public static void PublishWait<TEvent>(this IBus<IApplicationEvent> bus, TEvent message,
            TimeSpan? timeout = null)
            where TEvent : IApplicationEvent
        {
            bus.Publish(message);
            message.Wait(timeout);
        }

        public static void Publish<TMessage>(this IBus<IBusMessage> bus, TMessage message, Action completedCallback,
            TimeSpan? timeout = null)
            where TMessage : IBusMessage
        {
            message.Handled += completedCallback;
            bus.Publish(message);
        }
    }

    public class PlayPauseCommand : BusMessage
    {
    }

    public class PlayNextCommand : BusMessage
    {

    }

    public class PlayPreviousCommand : BusMessage
    {

    }

    public class StopCommand : BusMessage
    {

    }

    public class SetShuffleCommand : BusMessage
    {
        public bool Shuffle { get; private set; }

        public SetShuffleCommand(bool shuffle)
        {
            Shuffle = shuffle;
        }
    }

    public class SeekCommand : BusMessage
    {
        public double Offset { get; private set; }

        /// <summary>
        ///  </summary>
        /// <param name="offset">The offset in milliseconds to seek to</param>
        public SeekCommand(double offset)
        {
            Offset = offset;
        }
    }

    public class GetStateCommand : BusMessage<PlayingState>
    {
    }

    public class GetPlayingTrackCommand : BusMessage<ITrack>
    {
    }
}

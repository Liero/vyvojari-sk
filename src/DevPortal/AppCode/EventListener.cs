using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode
{
    public interface IEventListener
    {
        void SetHandled(Type eventHandler, Guid eventId);
        IEventHandlerAwaiter BeginListen(Type eventHandler, Guid eventId);
    }

    public interface IEventHandlerAwaiter : IDisposable
    {
        Task Wait(CancellationToken completionSource = default(CancellationToken));
    }

    public class EventListener: IEventListener
    {
        private List<EventHandlerAwaiter> awaiters = new List<EventHandlerAwaiter>();

        public IEventHandlerAwaiter BeginListen(Type eventHandler, Guid eventId)
        {
            var awaiter = new EventHandlerAwaiter(eventHandler, eventId);
            awaiter.Disposing += Awaiter_Disposing;
            lock (awaiters)
            {
                awaiters.Add(awaiter);
            }
            return awaiter;
        }

        public void SetHandled(Type eventHandler, Guid eventId)
        {
            lock (awaiters)
            {
                var matchingAwaiters = awaiters.Where(a => a.EventHandler == eventHandler && a.EventId == eventId)
                    .ToArray();

                foreach (var awaiter in matchingAwaiters)
                {
                    awaiter.SetHandled();
                    awaiter.Dispose();
                }
            }
        }

        private void Awaiter_Disposing(object sender, EventArgs e)
        {
            var awaiter = (EventHandlerAwaiter)sender;
            awaiter.Disposing -= Awaiter_Disposing;
            lock (awaiters)
            {
                awaiters.Remove(awaiter);
            }
        }
    }

    public class EventHandlerAwaiter : IEventHandlerAwaiter
    {
        public Type EventHandler { get; }
        public Guid EventId { get; }
        private readonly TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

        public event EventHandler Disposing;

        public EventHandlerAwaiter(Type eventHandler, Guid eventId)
        {
            EventHandler = eventHandler;
            EventId = eventId;
        }

        public void SetHandled()
        {
            tcs.SetResult(null);
        }

        public void Dispose()
        {
            tcs.TrySetCanceled();
            Disposing?.Invoke(this, EventArgs.Empty);
        }

        public Task Wait(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
            return tcs.Task;
        }
    }
}

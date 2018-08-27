using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.EventSourcing
{
    public interface IHandlerListener : IDisposable
    {
        Task Wait(CancellationToken completionSource = default(CancellationToken));
    }

    public class HandlerListener : IHandlerListener
    {
        public Type EventHandler { get; }
        public Guid EventId { get; }
        private readonly TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

        public event EventHandler Disposing;

        public HandlerListener(Type eventHandler, Guid eventId)
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

using DevPortal.Web.AppCode.EventSourcing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevPortal.Web.Mocks
{
    /// <summary>
    /// Returns HandlerListenerMocks, that does not wait for handlers.
    /// </summary>
    public class HandlerNotificationsStub : IHandlerNotifications
    {
        public IHandlerListener BeginListen(Type eventHandler, Guid eventId)
            => new HandlerListenerStub();

        public void SetHandled(Type eventHandler, Guid eventId) =>
            throw new NotImplementedException("This should not be called in a mock. Use real implementation instead");
    }

    public class HandlerListenerStub : IHandlerListener
    {
        public void Dispose() { }

        public Task Wait(CancellationToken completionSource)
            => Task.CompletedTask;
    }
}

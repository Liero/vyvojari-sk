using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.EventSourcing
{
    /// <summary>
    /// Rebus pipline will send notification, when eventhandler is invoked, so you can wait for the event
    /// </summary>
    public interface IHandlerNotifications
    {
        void SetHandled(Type eventHandler, Guid eventId);
        IHandlerListener BeginListen(Type eventHandler, Guid eventId);
    }

    public class HandlerNotififications: IHandlerNotifications
    {
        private List<HandlerListener> awaiters = new List<HandlerListener>();

        public IHandlerListener BeginListen(Type eventHandler, Guid eventId)
        {
            var awaiter = new HandlerListener(eventHandler, eventId);
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
            var awaiter = (HandlerListener)sender;
            awaiter.Disposing -= Awaiter_Disposing;
            lock (awaiters)
            {
                awaiters.Remove(awaiter);
            }
        }
    }
}

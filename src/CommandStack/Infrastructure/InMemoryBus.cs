using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DevPortal.CommandStack.Infrastructure
{
    public class InMemoryBus : IEventDispatcher
    {
        private readonly List<Type> _handlers = new List<Type>();
        private readonly Func<Type, object> activator;

        /// <param name="activator">Used to create instance of message handlers</param>
        public InMemoryBus(Func<Type, object> activator)
        {
            this.activator = activator;
        }

        public void RegisterHandler(Type type) => _handlers.Add(type);

        public void Dispatch(DomainEvent @event) => DeliverMessageToRegisteredHandlers(@event);

        private void DeliverMessageToRegisteredHandlers<T>(T message) where T : class
        {
            Type messageType = message.GetType();
            var openInterface = typeof(IHandleMessages<>);
            var closedInterface = openInterface.MakeGenericType(messageType);

            var handlersToNotify = from h in _handlers
                                   where closedInterface.GetTypeInfo().IsAssignableFrom(h.GetTypeInfo())
                                   select h;

            foreach (Type h in handlersToNotify)
            {
                var handlerInstance = activator(h);
                closedInterface.GetTypeInfo()
                    .GetMethod(nameof(IHandleMessages<T>.Handle))
                    .Invoke(handlerInstance, new[] { message });
                //dynamic handlerInstance = h.Value;
                //handlerInstance.Handle((dynamic)message);
            }
        }
    }
}

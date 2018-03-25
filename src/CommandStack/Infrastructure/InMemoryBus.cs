using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public Task Dispatch(DomainEvent @event) => DeliverMessageToRegisteredHandlers(@event);

        private async Task DeliverMessageToRegisteredHandlers<T>(T message) where T : class
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

                var handlerMethod = closedInterface.GetTypeInfo().GetMethod(nameof(IHandleMessages<T>.Handle));
                var task = (Task)handlerMethod.Invoke(handlerInstance, new[] { message });
                await task;
                //dynamic handlerInstance = h.Value;
                //handlerInstance.Handle((dynamic)message);
            }
        }
    }
}

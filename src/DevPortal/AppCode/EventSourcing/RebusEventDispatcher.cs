﻿
using System;
using System.Linq;
using System.Threading.Tasks;
using DevPortal.CommandStack.Infrastructure;
using Rebus.Bus;
using Rebus.Handlers;

namespace DevPortal.Web.AppCode.EventSourcing
{
    public class RebusEventDispatcher : IEventDispatcher
    {
        public RebusEventDispatcher(IBus bus)
        {
            this._bus = bus;
        }
        private readonly IBus _bus;

        public async Task Dispatch(DomainEvent @event)
        {
            await _bus.Send(@event);
        }

        public void RegisterHandler(Type handler)
        {
            var eventTypes = handler
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleMessages<>))
                .SelectMany(i => i.GetGenericArguments())
                .ToArray();

            foreach (var eventType in eventTypes)
            {
                _bus.Subscribe(eventType);
            }              
        }
    }
}

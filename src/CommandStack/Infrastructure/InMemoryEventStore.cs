using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevPortal.CommandStack.Infrastructure
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<DomainEvent> _events;
        IEventDispatcher _eventDispatcher;

        public InMemoryEventStore(IEventDispatcher eventDispatcher)
        {
            _events = new List<DomainEvent>(1000);
            _eventDispatcher = eventDispatcher;
        }

        public void Save(DomainEvent @event)
        {
            _events.Add(@event);
            _eventDispatcher.Dispatch(@event);
        }

        /// <summary>
        /// Retrieves all events of a type which satisfy a condition
        /// </summary>
        /// <typeparam name="T">The type of the events to retrieve</typeparam>
        /// <param name="filter">The condition events must satisfy in order to be retrieved.</param>
        /// <returns>The events which satisfy the specified condition</returns>
        public IEnumerable<T> Find<T>(Func<T, bool> filter) where T : DomainEvent
        {
            return _events
                .Where(evt => evt.GetType() == typeof(T))
                .Cast<T>()
                .Where(filter);
        }
    }
}

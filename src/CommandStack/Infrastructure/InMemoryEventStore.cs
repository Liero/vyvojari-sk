using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.CommandStack.Infrastructure
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<EventWrapper> _events;
        IEventDispatcher _eventDispatcher;

        public InMemoryEventStore(IEventDispatcher eventDispatcher)
        {
            _events = new List<EventWrapper>(1000);
            _eventDispatcher = eventDispatcher;
        }

        public void Save(DomainEvent @event)
        {
            lock (_events)
            {
                _events.Add(new EventWrapper { Event = @event });
            }
            _eventDispatcher.Dispatch(@event);
        }

        /// <summary>
        /// Retrieves all events of a type which satisfy a condition
        /// </summary>
        /// <typeparam name="T">The type of the events to retrieve</typeparam>
        /// <param name="filter">The condition events must satisfy in order to be retrieved.</param>
        /// <returns>The events which satisfy the specified condition</returns>
        public IEnumerable<T> Find<T>(Func<EventWrapper, bool> filter, int limit) where T : DomainEvent
        {
            lock (_events)
            {
                var query = _events.Where(e => e.Event is T)
                    .Where(filter);

                if (limit > 0)
                {
                    query = query.Take(limit);
                }

                return _events
                    .Select(e => (T)e.Event)
                    .ToArray();
            }
        }
    }
}

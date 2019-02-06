using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.CommandStack.Infrastructure
{
    public interface IEventStore
    {
        /// <summary>
        /// Saves an event within the store
        /// </summary>
        /// <param name="event">The event</param>
        void Save(DomainEvent @event);

        /// <summary>
        /// Retrieves all events of a type which satisfy a condition
        /// </summary>
        /// <typeparam name="T">The type of the events to retrieve</typeparam>
        /// <param name="filter">The condition events must satisfy in order to be retrieved.</param>
        /// <returns>The events which satisfy the specified condition</returns>
        IEnumerable<T> Find<T>(Func<EventWrapper, bool> filter, int limit = 0) where T : DomainEvent;
    }
}

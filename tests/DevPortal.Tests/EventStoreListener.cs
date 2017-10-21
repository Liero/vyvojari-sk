using DevPortal.CommandStack.Events;
using DevPortal.CommandStack.Infrastructure;
using Moq;
using System;

namespace DevPortal.Web
{
    /// <summary>
    /// Listens for events saved in EventStore
    /// </summary>
    public class EventStoreListener<T> where T : DomainEvent
    {
        readonly Mock<IEventStore> _eventStoreMock;
        T actualEvent;

        public EventStoreListener(Mock<IEventStore> eventStoreMock)
        {
            _eventStoreMock = eventStoreMock;
            _eventStoreMock
                .Setup(es => es.Save(It.IsAny<T>()))
                .Callback<DomainEvent>(evt => actualEvent = (T)evt);
        }

        /// <summary>
        /// verifies event is saved exactly once witch specified parameters;
        /// </summary>
        public void VerifySavedOnce(Action<T> _assert)
        {
            _eventStoreMock.Verify(es => es.Save(It.IsAny<T>()), Times.Once);
            _assert(actualEvent);
        }
    }
}

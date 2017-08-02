using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevPortal.CommandStack.Events;
using System.Linq;
using Moq;

namespace DevPortal.CommandStack.Infrastructure
{
    [TestClass]
    public class InMemoryEventStoreTests
    {
        [TestMethod]
        public void InMemoryEventStore_SavedEventCanBeFound()
        {
            //setup
            IEventStore eventStore = new InMemoryEventStore(new Mock<IEventDispatcher>().Object);
            Guid testEventId = Guid.NewGuid();

            //action
            eventStore.Save(new TestEvent
            {
                TestEventId = testEventId
            });

            var evt = eventStore.Find<TestEvent>(i => i.TestEventId == testEventId)
                .Single();

            Assert.IsTrue(evt.TimeStamp > DateTime.Now.AddSeconds(-1), "TimeStamp");
            Assert.AreEqual(evt.TestEventId, testEventId);
        }

        [TestMethod]
        public void InMemoryEventStore_SavedEventIsDispatched()
        {
            //setup
            var eventDispatcherMock = new Mock<IEventDispatcher>();
            eventDispatcherMock.Setup(dispatcher =>
                dispatcher.Dispatch(It.IsAny<DomainEvent>()));

            IEventStore eventStore = new InMemoryEventStore(eventDispatcherMock.Object);
            DomainEvent @event = new TestEvent();

            //action
            eventStore.Save(@event);

            //assert
            eventDispatcherMock
                .Verify(dispatcher => dispatcher.Dispatch(@event), Times.Once);
        }
    }


}


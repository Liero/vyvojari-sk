using DevPortal.CommandStack.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevPortal.CommandStack.Infrastructure
{
    [TestClass]
    public class InMemoryBusTests
    {

        [TestMethod]
        public void InMemoryEventDispatcher_DispatchMessagesToRegisteredHandler()
        {
            //setup
            TestEvent testEvent = new TestEvent();
            AnotherEvent anotherEvent = new AnotherEvent();
            int testEventsHandled = 0;
            int anotherEventsHandled = 0;
            var handler = new EventHandlerStub();
            handler.HandleTestEventCallback = message =>
            {
                testEventsHandled++;
                Assert.AreEqual(testEvent, message);
            };
            handler.HandleAnotherEventCallback = message =>
            {
                anotherEventsHandled++;
                Assert.AreEqual(anotherEvent, message);
            };

            InMemoryBus eventDispatcher = new InMemoryBus(t => handler);
            eventDispatcher.RegisterHandler<EventHandlerStub>();
            

            //action
            eventDispatcher.Dispatch(testEvent);
            eventDispatcher.Dispatch(anotherEvent);

            //assert
            Assert.AreEqual(1, testEventsHandled);
            Assert.AreEqual(1, anotherEventsHandled);
        }
    }

    internal class EventHandlerStub : IHandleMessages<TestEvent>, IHandleMessages<AnotherEvent>
    {
        public Action<TestEvent> HandleTestEventCallback { private get; set; }
        public Action<AnotherEvent> HandleAnotherEventCallback { private get; set; }

        void IHandleMessages<AnotherEvent>.Handle(AnotherEvent message) => HandleAnotherEventCallback?.Invoke(message);
        void IHandleMessages<TestEvent>.Handle(TestEvent message) => HandleTestEventCallback?.Invoke(message);
    }

}

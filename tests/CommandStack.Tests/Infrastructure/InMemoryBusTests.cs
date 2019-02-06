using DevPortal.CommandStack.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
                return Task.CompletedTask;
            };
            handler.HandleAnotherEventCallback = message =>
            {
                anotherEventsHandled++;
                Assert.AreEqual(anotherEvent, message);
                return Task.CompletedTask;
            };

            InMemoryBus eventDispatcher = new InMemoryBus(t => handler);
            eventDispatcher.RegisterHandler(typeof(EventHandlerStub));
            

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
        public Func<TestEvent, Task> HandleTestEventCallback { private get; set; }
        public Func<AnotherEvent, Task> HandleAnotherEventCallback { private get; set; }


        Task IHandleMessages<AnotherEvent>.Handle(AnotherEvent message) => HandleAnotherEventCallback?.Invoke(message);
        Task IHandleMessages<TestEvent>.Handle(TestEvent message) => HandleTestEventCallback?.Invoke(message);
    }

}

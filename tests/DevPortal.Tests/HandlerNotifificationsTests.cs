using DevPortal.CommandStack.Infrastructure;
using DevPortal.Web.AppCode;
using DevPortal.Web.AppCode.EventSourcing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.Web
{
    [TestClass]
    public class HandlerNotifificationsTests
    {
        IHandlerNotifications eventHandlerNotifications;

        [TestMethod]
        public async Task HandlerListener_Wait_WaitsUntilEventIsHandled()
        {
            eventHandlerNotifications = new HandlerNotififications();
            var evt = new MyEvent { Counter = 1 };
            using (var handlerListener = eventHandlerNotifications.BeginListen(typeof(MyEventHandler), evt.Id))
            {
                var task =  handlerListener.Wait(new System.Threading.CancellationTokenSource(3000).Token);

                await Task.Delay(1);
                Assert.IsFalse(task.IsCompleted);
                eventHandlerNotifications.SetHandled(typeof(MyEventHandler), evt.Id);

                await task;
                Assert.IsTrue(task.IsCompletedSuccessfully);

            }
        }


        private class MyEventHandler : IHandleMessages<MyEvent>
        {
            public Task Handle(MyEvent message)
            {
                return Task.CompletedTask;
            }
        }

        public class MyEvent : DomainEvent
        {
            public int Counter { get; set; }
        }
    }
}

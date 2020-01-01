using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevPortal.Web.NewEventSourcing
{
    [TestClass]
    public class EventsForwarderTests
    {
        ServiceProvider _provider;

        [TestInitialize]
        public void Init()
        {
            var services = new ServiceCollection();
            services.AddDbContext<DevPortalDbContext>(options => options.UseInMemoryDatabase("DevPortalDbContext"));
            services.AddDbContext<EventsDbContext>(options => options.UseInMemoryDatabase("EventsDbContext"));
            _provider = services.BuildServiceProvider();
            Denormalizer1.HandledEvents.Clear();
        }

        [TestMethod]
        public void EventsForwarder_ForwardsEventToDenormalizerInvoker()
        {
            var eventDispatcherMock = new Mock<IEventDispatcher>();
            eventDispatcherMock.Setup(e => e.Dispatch(It.IsAny<DomainEvent>()))
                .Returns(Task.CompletedTask);

            var sqlEventsStore = new SqlEventStore(
               eventDispatcherMock.Object,
               _provider.GetRequiredService<DbContextOptions<EventsDbContext>>(),
               new Type[] { typeof(Evt1) }
               );

            sqlEventsStore.Save(new Evt1("Hello"));
            sqlEventsStore.Save(new Evt1("World"));

            var eventsForwarder = new EventsForwarder(
                "AllDenormalizers",
                new[] { typeof(Denormalizer1) },
                _provider,
                sqlEventsStore);

            eventsForwarder.Start().GetAwaiter();

            sqlEventsStore.Save(new Evt1("!"));
            eventsForwarder.Start().GetAwaiter();

            Task.Run(() =>
            {
                while (Denormalizer1.HandledEvents.Count < 3)
                {
                    Thread.Sleep(10);
                }
            }).Wait(1000);

            Assert.AreEqual(3, Denormalizer1.HandledEvents.Count);
            Assert.AreEqual("Hello", Denormalizer1.HandledEvents[0].Title);
            Assert.AreEqual("World", Denormalizer1.HandledEvents[1].Title);
            Assert.AreEqual("!", Denormalizer1.HandledEvents[2].Title);
        }

        //fakes


        private class Denormalizer1
        {
            public static List<Evt1> HandledEvents { get; } = new List<Evt1>();

            public Task Handle(Evt1 evt)
            {
                HandledEvents.Add(evt);
                return Task.CompletedTask;
            }
        }
    }

    public class Evt1 : DomainEvent
    {
        public Evt1(string title)
        {
            Title = title;
        }

        public string Title { get; }
    }
}

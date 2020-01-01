using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.Web.NewEventSourcing
{
    [TestClass]
    public class EventStoreEventForwarderIntergrationTest
    {
        ServiceProvider _provider;

        [TestInitialize]
        public void Init()
        {
            var services = new ServiceCollection();
            services.AddDbContext<DevPortalDbContext>(options => options.UseInMemoryDatabase("DevPortalDbContext"));
            services.AddDbContext<EventsDbContext>(options => options.UseInMemoryDatabase("EventsDbContext"));
            _provider = services.BuildServiceProvider();
            Denormalizer1.Reset();
        }

        [TestMethod]
        public async Task EventStoreForwardEventsToEventForwarder()
        {
            //1. create two instances of EventStore (sharing the same storage)
            //2. save few Hello and World events to diferent EventSTores
            //3. Setup event forwarder to listen on first instance of EventStore;
            //4. Save events 


            var eventDispatcherMock = new Mock<IEventDispatcher>();
            eventDispatcherMock.Setup(e => e.Dispatch(It.IsAny<DomainEvent>()))
                .Returns(Task.CompletedTask);

            var sqlEventsStore = new SqlEventStore(
              eventDispatcherMock.Object,
              _provider.GetRequiredService<DbContextOptions<EventsDbContext>>(),
              new Type[] { typeof(Evt1) }
              );

            var sqlEventsStore2 = new SqlEventStore(
              eventDispatcherMock.Object,
              _provider.GetRequiredService<DbContextOptions<EventsDbContext>>(),
              new Type[] { typeof(Evt1) }
              );

            sqlEventsStore.Save(new Evt1("Hello"));
            sqlEventsStore2.Save(new Evt1("World"));


            var eventsForwarder = new EventsForwarder(
                "AllDenormalizers",
                new[] { typeof(Denormalizer1) },
                _provider,
                sqlEventsStore);

            sqlEventsStore.EventSaved += (o, evt) => eventsForwarder.Start().GetAwaiter();
            sqlEventsStore2.EventSaved += (o, evt) => eventsForwarder.Start().GetAwaiter();

            sqlEventsStore2.Save(new Evt1("Event Sourcing"));
            sqlEventsStore.Save(new Evt1("!"));

            var evts = await Denormalizer1.HandledEventObservable
                .OfType<Evt1>()
                .Take(4)
                .TakeUntil(DateTimeOffset.Now.AddSeconds(1))
                .ToArray();

            CollectionAssert.AreEqual(new[] { "Hello", "World", "Event Sourcing", "!" }, evts.Select(e => e.Title).ToArray());

        }



        private class Denormalizer1
        {
            public static DateTime Start;
            public static ReplaySubject<DomainEvent> HandledEventObservable { get; private set; }

            public static void Reset()
            {
                Start = DateTime.UtcNow;
                HandledEventObservable = new ReplaySubject<DomainEvent>();
            }

            public Task Handle(Evt1 evt)
            {
                DateTime now = DateTime.UtcNow;
                Trace.WriteLine($"Event '{evt.Title}' - {evt.TimeStamp.TimeOfDay},\t handled at {now.TimeOfDay}.\t Delay {now - evt.TimeStamp}\f from start: {now - Start}");
                HandledEventObservable.OnNext(evt);
                return Task.CompletedTask;
            }
        }
    }
}

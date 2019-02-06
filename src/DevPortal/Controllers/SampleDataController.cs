using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevPortal.CommandStack.Infrastructure;
using DevPortal.Web.AppCode.EventSourcing;
using DevPortal.QueryStack.Denormalizers;
using Rebus.Handlers;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DevPortal.Web.Controllers
{
    public class SampleDataController : Controller
    {
        private readonly IEventStore _eventStore;
        private readonly IHandlerNotifications _handlerNotifications;
        private readonly ILogger _logger;
        int counter;
        public SampleDataController(IEventStore eventStore, IHandlerNotifications notifications, ILoggerFactory logger)
        {
            _eventStore = eventStore;
            _handlerNotifications = notifications;
            _logger = logger.CreateLogger("SampleDataController");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> All()
        {
            var sampleData = new DesignTimeData.SampleEventsGenerator();
            counter = 0;
            foreach (var evt in sampleData.News(10))
            {

                await SaveAndWait(evt);
                counter++;
            }
            foreach (var evt in sampleData.Forum(20))
            {
                await SaveAndWait(evt);
                counter++;
            }
            foreach (var evt in sampleData.Blog(10))
            {
                await SaveAndWait(evt);
                counter++;
            }
            ViewBag.Message = $"Generated {counter} events";
            return View(nameof(Index));
        }

        public async Task SaveAndWait(DomainEvent evt)
        {
            var handlers = GetDenormalizers(evt.GetType());
            var listeners = handlers
                .Select(handler => _handlerNotifications.BeginListen(handler, evt.Id))
                .ToArray();

            try
            {
                Stopwatch sw = Stopwatch.StartNew();

                _logger.LogInformation($"Counter: {counter}\tSaving {evt.GetType().Name} ID {evt.Id}");
                _eventStore.Save(evt);
                _logger.LogInformation($"Counter: {counter}\tSaved {evt.GetType().Name} ID {evt.Id}");

                foreach (var listener in listeners)
                {
                    _logger.LogInformation($"Counter: {counter}\tWaiting for {evt.GetType().Name} ID {evt.Id}");

                    await listener.Wait();
                }

                sw.Stop();
                Trace.WriteLine($"Saving {evt.GetType().Name} took\t{sw.ElapsedMilliseconds} miliseconds");
            }
            finally
            {
                foreach (var listener in listeners) listener.Dispose();
            }
        }



        static Dictionary<Type, List<Type>> EventToDenormalizers = new Dictionary<Type, List<Type>>();

        static internal List<Type> GetDenormalizers(Type typeOfEvent)
        {
            if (!EventToDenormalizers.TryGetValue(typeOfEvent, out var result))
            {
                result = EventSourcingConfig.AllDenormalizers
                    .Where(handler => handler.GetInterfaces().Any(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(IHandleMessages<>)
                        && i.GenericTypeArguments[0].IsAssignableFrom(typeOfEvent)
                    ))
                    .ToList();

                EventToDenormalizers.Add(typeOfEvent, result);
            }
            return result;
        }
    }
}

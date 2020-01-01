using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.QueryStack
{
    /// <summary>
    /// Forwards events from EventStore to denormalizer and tracks current position (last handled event)
    /// </summary>
    public class EventsForwarder
    {
        private static object locker = new object();
        private static bool _isRunning = false;

        public string ForwarderKey { get; }
        public Type[] DenormalizerTypes { get; }
        DateTime _lastStarted;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventStore _eventStore;
        private DenormalizerState _denormalizerState;

        public EventsForwarder(
             string denormalizerKey,
             Type[] denormalizerTypes,
             IServiceProvider serviceProvider,
             IEventStore eventStore)
        {
            ForwarderKey = denormalizerKey;
            DenormalizerTypes = denormalizerTypes;
            _serviceProvider = serviceProvider;
            _eventStore = eventStore;
        }

        /// <summary>
        /// Start forwarding new events stored in database until all events are forwarded and then stops. 
        /// After that, Start method has to be called again in order to forward newer events
        /// </summary>
        public async Task Start()
        {
            lock (locker)
            {
                _lastStarted = DateTime.UtcNow;
                if (_isRunning) return;
                _isRunning = true;
            }
            await ForwardEvents();
        }

        protected async Task ForwardEvents()
        {
            var sw = new Stopwatch();
            while (_isRunning)
            {
                sw.Restart();
                var newEvents = (await GetNewEvents()).ToArray();

                foreach (var newEvent in newEvents)
                {
                    await ForwardEvent(newEvent);
                }

                lock (locker)
                {
                    if (newEvents.Length > 0)
                    {
                        Console.WriteLine($"Processed {newEvents.Length} events in {sw.ElapsedMilliseconds / 1000d}: {newEvents.Length * 1000d / sw.ElapsedMilliseconds} events/sec");
                    }
                    else if (_lastStarted < DateTime.UtcNow)
                    {
                        _isRunning = false;
                        break;
                    }
                }
            }
        }

        protected async Task ForwardEvent(EventWrapper wrapper)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<DevPortalDbContext>())
                {
                    //invoke all denormalizers and update denormalizer state in the same UoW
                    foreach (var denormalizerType in DenormalizerTypes)
                    {
                        //todo: performanceImproovements: cache handlerInvoked for each event type to avoid too many DenormalizerInvoker instaces being created
                        var invoker = new DenormalizerInvoker(denormalizerType, scope.ServiceProvider);
                        bool handlerInvoked = await invoker.Invoke(wrapper.Event);
                    }
                    UpdateState(wrapper, dbContext);
                    await dbContext.SaveChangesAsync();
                }
            }
        }



        public async Task<IEnumerable<EventWrapper>> GetNewEvents()
        {
            await InitializeState();
            var events = _eventStore.Find<DomainEvent>(e => e.EventNumber > _denormalizerState.EventNumber, 100);
            return events;
        }


        public async Task InitializeState()
        {
            using (var dbContext = ActivatorUtilities.CreateInstance<DevPortalDbContext>(_serviceProvider))
            {
                _denormalizerState = await dbContext.Denormalizers.FindAsync(ForwarderKey);
                if (_denormalizerState == null)
                {
                    _denormalizerState = new DenormalizerState
                    {
                        Key = ForwarderKey,
                    };
                    dbContext.Add(_denormalizerState);
                    await dbContext.SaveChangesAsync();
                }
            }
        }


        private void UpdateState(EventWrapper evtWrapper, DevPortalDbContext dbContext)
        {
            var state = this._denormalizerState;
            dbContext.Attach(state);
            state.EventNumber = evtWrapper.EventNumber;
            state.Timestamp = evtWrapper.TimeStamp;
        }

    }
}

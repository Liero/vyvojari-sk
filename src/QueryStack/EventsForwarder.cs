using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack.Model;
using System;
using System.Collections.Generic;
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
        public Type DenormalizerType { get; }
        DateTime _lastStarted;
        private readonly Func<DevPortalDbContext> _dbContextFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventStore _eventStore;
        private DenormalizerState _denormalizerState;

        public EventsForwarder(
             Func<DevPortalDbContext> dbContextFactory,
             Type denormalizerType,
             IServiceProvider serviceProvider,
             IEventStore eventStore)
        {
            _dbContextFactory = dbContextFactory;
            DenormalizerType = denormalizerType;
            _serviceProvider = serviceProvider;
            _eventStore = eventStore;
        }

        /// <summary>
        /// Start forwarding new events stored in database until all events are forwarded and then stops. 
        /// After that, Start method has to be called again in order to forward newer events
        /// </summary>
        public async void Start()
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
            while (true)
            {
                var newEvents = await GetNewEvents();

                lock (locker)
                {
                    if (!newEvents.Any() && _lastStarted < DateTime.UtcNow)
                    {
                        _isRunning = false;
                        break;
                    }
                }

                foreach (var newEvent in newEvents)
                {
                    await InvokeDenormaizer(newEvent);         
                }
            }
        }

        protected async Task InvokeDenormaizer(DomainEvent evt)
        {
            //invoke denormalizer and update denormalizer state in the same UoW
            using (var dbContext = _dbContextFactory())
            {
                DenormalizerInvoker invoker = new DenormalizerInvoker(
                    DenormalizerType,
                    _serviceProvider,
                    dbContext);
                var handlerInvoked = await invoker.Invoke(evt);
                if (handlerInvoked)
                {
                    UpdateState(evt, dbContext);
                }
                await dbContext.SaveChangesAsync();
            }
        }


        public async Task<IEnumerable<DomainEvent>> GetNewEvents()
        {
            var state = await GetState();
            var events = _eventStore.Find<DomainEvent>(e => e.TimeStamp >= state.Timestamp, 10)
                .SkipWhile(e => e.TimeStamp <= state.Timestamp && e.Id != state.EventId)
                .Where(e => e.Id != state.EventId);
            return events;
        }


        public async Task<DenormalizerState> GetState()
        {
            if (_denormalizerState != null) return _denormalizerState;
            using (var dbContext = _dbContextFactory())
            {
                _denormalizerState = await dbContext.Denormalizers.FindAsync(DenormalizerType.Name);
                if (_denormalizerState == null)
                {
                    _denormalizerState = new DenormalizerState
                    {
                        TypeName = DenormalizerType.Name,
                    };
                    dbContext.Add(_denormalizerState);
                    await dbContext.SaveChangesAsync();
                };
            }
            return _denormalizerState;
        }


        private void UpdateState(DomainEvent evt, DevPortalDbContext dbContext)
        {
            dbContext.Attach(_denormalizerState);
            _denormalizerState.EventId = evt.Id;
            _denormalizerState.Timestamp = evt.TimeStamp;
        }

    }
}

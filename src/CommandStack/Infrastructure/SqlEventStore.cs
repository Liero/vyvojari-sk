using DevPortal.CommandStack.Events;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevPortal.CommandStack.Infrastructure
{
    /// <summary>
    /// EventWrapper is needed only until EF Core 2.1. 
    /// </summary>
    public class EventWrapper
    {     
        public Guid Id { get; internal set; }
        public long EventNumber { get; private set; }
        public DateTime TimeStamp { get; internal set; }
        public Guid? ForumThreadId { get; internal set; }
        public Guid? BlogId { get; internal set; }
        public Guid? NewsItemId { get; internal set; }
        public Guid? MessageId { get; internal set; }
        
        public string EventType { get; internal set; }
        public string SerializedEvent { get; internal set; }
        public DomainEvent Event { get; internal set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EventsDbContext : DbContext
    {
        public EventsDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<EventWrapper> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddDateTimeUtcConversion();

            modelBuilder.Entity<EventWrapper>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EventNumber).ValueGeneratedOnAdd();
                entity.HasAlternateKey(e => e.EventNumber).ForSqlServerIsClustered();

                entity.Property(e => e.EventType).IsRequired();
                entity.HasIndex(e => e.EventType);

                entity.Property(e => e.TimeStamp).IsRequired();
                entity.HasIndex(e => e.TimeStamp);
                entity.Property(e => e.SerializedEvent).IsRequired();
                entity.Ignore(e => e.Event);

                entity.HasIndex(e => e.ForumThreadId);
                entity.HasIndex(e => e.BlogId);
                entity.HasIndex(e => e.NewsItemId);
                entity.HasIndex(e => e.MessageId);


                entity.Property(e => e.ForumThreadId).IsRequired(false);
                entity.Property(e => e.BlogId).IsRequired(false);
                entity.Property(e => e.NewsItemId).IsRequired(false);
            });
        }
    }

    public class SqlEventStore : IEventStore, IDisposable
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly Type[] _registeredEvents;
        private readonly EventsDbContext _dbContext;

        public SqlEventStore(
            IEventDispatcher eventDispatcher,
            DbContextOptions<EventsDbContext> dbContextOptions,
            Type[] registeredEvents)
        {
            _eventDispatcher = eventDispatcher;
            _dbContext = new EventsDbContext(dbContextOptions);
            _registeredEvents = registeredEvents;
        }

        public event EventHandler<DomainEvent> EventSaved;

        public IEnumerable<EventWrapper> Find<T>(Expression<Func<EventWrapper, bool>> filter, int limit) where T : DomainEvent
        {
            var possibleEventTypes = _registeredEvents.Where(e => typeof(T).IsAssignableFrom(e))
                .Select(e => e.Name)
                .ToArray();

            IQueryable<EventWrapper> query = _dbContext.Events.AsNoTracking()
                .Where(filter)
                .Where(wrapper => possibleEventTypes.Contains(wrapper.EventType))
                .OrderBy(wrapper => wrapper.EventNumber);

            if (limit > 0) query = query.Take(limit);
            foreach (var eventWrapper in query)
            {
                var eventType = _registeredEvents.FirstOrDefault(e => e.Name == eventWrapper.EventType);
                eventWrapper.Event = (T)JsonConvert.DeserializeObject(eventWrapper.SerializedEvent, eventType);
                yield return eventWrapper;
            };
        }

        public void Save(DomainEvent @event)
        {
            var wrapper = new EventWrapper
            {
                Id = @event.Id,
                EventType = @event.GetType().Name,
                TimeStamp = @event.TimeStamp
            };

            wrapper.SerializedEvent = JsonConvert.SerializeObject(@event);
            wrapper.Event = @event;

            MapEventPropertiesToWrapperProperties(@event, wrapper);

            _dbContext.Events.Add(wrapper);
            _dbContext.SaveChanges();
            _dbContext.Entry(wrapper).State = EntityState.Detached;
            EventSaved?.Invoke(this, @event);
            _eventDispatcher.Dispatch(@event).Wait(); //todo: deal better with eventual inconsistency
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        /// <summary>
        /// EventWrapper can contain additional properties, that are used for filtering.
        /// </summary>
        protected virtual void MapEventPropertiesToWrapperProperties(DomainEvent @event, EventWrapper wrapper)
        {
            var mappedProperties = from wrapperProp in wrapper.GetType().GetProperties()
                                   join eventProp in @event.GetType().GetProperties() on wrapperProp.Name equals eventProp.Name
                                   where wrapperProp.PropertyType.IsAssignableFrom(eventProp.PropertyType)
                                   select new { wrapperProp, eventProp };

            foreach (var propPair in mappedProperties)
            {
                var value = propPair.eventProp.GetValue(@event);
                propPair.wrapperProp.SetValue(wrapper, value);
            }
        }

        static IEnumerable<Type> SelfAndBaseTypes(Type type)
        {
            do
            {
                yield return type;
                type = type.BaseType;
            }
            while (type != null);
        }

  
    }
}

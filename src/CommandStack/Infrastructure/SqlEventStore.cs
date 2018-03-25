using DevPortal.CommandStack.Events;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid? ForumThreadId { get; set; }
        public Guid? BlogId { get; set; }
        public Guid? NewsItemId { get; set; }
        public string EventType { get; set; }
        internal string SerializedEvent { get; set; }

        private DomainEvent _event;
        public DomainEvent Event
        {
            get
            {
                if (_event == null && SerializedEvent != null)
                {
                    Type eventType = Type.GetType(SerializedEvent);
                    _event = (DomainEvent)JsonConvert.DeserializeObject(SerializedEvent, Type.GetType(EventType));
                }
                return _event;
            }
            set
            {
                _event = value;
                SerializedEvent = JsonConvert.SerializeObject(value);
                EventType = value.GetType().FullName;
                Id = Event.Id;
                TimeStamp = Event.TimeStamp;

                var wrapperProps = this.GetType().GetProperties().ToDictionary(p => p.Name);

                foreach (var prop in value.GetType().GetProperties())
                {
                    if (wrapperProps.TryGetValue(prop.Name, out PropertyInfo wrapperProp)
                        && wrapperProp.PropertyType.IsAssignableFrom(prop.PropertyType))
                    {
                        wrapperProp.SetValue(this, prop.GetValue(value));
                    }
                }
                this.Id = value.Id;
                this.TimeStamp = value.TimeStamp;
            }
        }
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

            modelBuilder.Entity<EventWrapper>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EventType).IsRequired();
                entity.HasIndex(e => e.EventType);

                entity.Property(e => e.TimeStamp).IsRequired();
                entity.HasIndex(e => e.TimeStamp);
                entity.Property(e => e.SerializedEvent).IsRequired();
                entity.Ignore(e => e.Event);

                entity.HasIndex(e => e.ForumThreadId);
                entity.HasIndex(e => e.BlogId);
                entity.HasIndex(e => e.NewsItemId);


                entity.Property(e => e.ForumThreadId).IsRequired(false);
                entity.Property(e => e.BlogId).IsRequired(false);
                entity.Property(e => e.NewsItemId).IsRequired(false);
            });
        }
    }

    public class SqlEventStore : IEventStore, IDisposable
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly EventsDbContext _dbContext;

        public SqlEventStore(IEventDispatcher eventDispatcher, EventsDbContext dbContext)
        {
            _eventDispatcher = eventDispatcher;
            _dbContext = dbContext;
        }

        public IEnumerable<T> Find<T>(Func<EventWrapper, bool> filter) where T : DomainEvent
        {
            var eventTypeName = typeof(T).FullName;
            return _dbContext.Events.AsNoTracking()
                .Where(wrapper => wrapper.EventType == eventTypeName)
                .AsEnumerable()
                .Select(t => (T)t.Event);
        }

        public void Save(DomainEvent @event)
        {
            var wrapper = new EventWrapper();
            wrapper.Event = @event;
            _dbContext.Events.Add(wrapper);
            _dbContext.SaveChanges();
            _eventDispatcher.Dispatch(@event).Wait(); //todo: deal better with eventual inconsistency
        }


        public void Dispose()
        {
            _dbContext.SaveChanges();
        }
    }
}

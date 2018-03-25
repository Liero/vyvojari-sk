using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack.Denormalizers;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;


namespace DevPortal.Web.AppCode.Startup
{
    public static class EventSourcingConfig
    {
        public static void AddInMemoryEventSourcing(this IServiceCollection services)
        {

            services.AddRebus(configure => configure
              .Logging(l => l.Trace())
              .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "Messages"))
              .Routing(r => r.TypeBased().MapAssemblyOf<DomainEvent>("Messages")));


            services.AddSingleton<IEventDispatcher, RebusEventDispatcher>();
            services.AutoRegisterHandlersFromAssemblyOf<ActivityDenormalizer>();
            services.AddSingleton<IEventStore, SqlEventStore>();
            //services.AddSingleton<IEventDispatcher>(serviceProvider =>
            //{

            //    var eventDispatcher = new InMemoryBus(type =>
            //        ActivatorUtilities.CreateInstance(serviceProvider, type));
            //    RegisterHandlers(eventDispatcher);
            //    return eventDispatcher;
            //});
            //services.AddSingleton<IEventStore, InMemoryEventStore>();
        }

        public static void AddSqlEventSourcing(this IServiceCollection services)
        {
            AddInMemoryEventSourcing(services);
        }



        public static void RegisterHandlers(this IEventDispatcher eventDispatcher)
        {
            foreach (var denormalizer in AllDenormalizers)
            {
                eventDispatcher.RegisterHandler(denormalizer);
            }
        }

        private static IEnumerable<Type> AllDenormalizers => Assembly.GetAssembly(typeof(NewsItemDenormalizer))
                .GetExportedTypes()
                .Where(t => t.Name.EndsWith("Denormalizer"));

    }
}

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
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;
using Rebus.Activation;
using DevPortal.Web.AppCode.Extensions;

namespace DevPortal.Web.AppCode.Startup
{
    public static class EventSourcingConfig
    {
        public static void AddInMemoryEventSourcing(this IServiceCollection services)
        {
            services.AddRebus(configure => configure
              .Logging(l => l.Trace())
              .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "Messages"))
              .Routing(r => r.TypeBased().MapAssemblyOf<DomainEvent>("Messages"))
              .Options(configurer =>
                {
                    configurer.Decorate<IPipeline>(c =>
                    {
                        var pipeline = c.Get<IPipeline>();
                        var step = new MyActivateHandlersStep(c.Get<IHandlerActivator>());
                        return new PipelineStepInjector(pipeline).OnReceive(step, PipelineRelativePosition.Before, typeof(ActivateHandlersStep));
                    });
                    configurer.Decorate<IPipeline>(c =>
                    {
                        var pipeline = c.Get<IPipeline>();
                        return new PipelineStepRemover(pipeline).RemoveIncomingStep(s => s.GetType() == typeof(ActivateHandlersStep));
                    });
                })
              );



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

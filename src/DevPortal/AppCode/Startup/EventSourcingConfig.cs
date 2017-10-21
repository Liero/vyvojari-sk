using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack.Denormalizers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Startup
{
    public static class EventSourcingConfig
    {
        public static void AddEventSourcing(this IServiceCollection services)
        {
            services.AddSingleton<IEventDispatcher>(serviceProvider =>
            {
                
                var eventDispatcher = new InMemoryBus(type =>
                    ActivatorUtilities.CreateInstance(serviceProvider, type));
                RegisterHandlers(eventDispatcher);
                return eventDispatcher;
            });
            services.AddSingleton<IEventStore, InMemoryEventStore>();
        }

        private static void RegisterHandlers(IEventDispatcher eventDispatcher)
        {
            //eventDispatcher.RegisterHandler<NewsItemDenormalizer>();
            //eventDispatcher.RegisterHandler<BlogDenormalizer>();
            //eventDispatcher.RegisterHandler<ForumThreadDenormalizer>();

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

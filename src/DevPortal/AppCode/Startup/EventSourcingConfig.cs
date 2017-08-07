using DevPortal.CommandStack.Infrastructure;
using DevPortal.QueryStack.Denormalizers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.Startup
{
    public static class EventSourcingConfig
    {
        public static void AddEventSourcing(this IServiceCollection services)
        {
            InMemoryBus eventBus = new InMemoryBus();
            eventBus.RegisterHandler<NewsItemDenormalizer>();
            services.AddSingleton<IEventDispatcher>(eventBus);
            //todo: register handlers;

            InMemoryEventStore eventStore = new InMemoryEventStore(eventBus);
            services.AddSingleton<IEventStore>(eventStore);
        }
    }
}

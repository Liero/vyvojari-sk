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
            services.AddSingleton<IEventDispatcher>(serviceProvider =>
            {
                var eventDispatcher = new InMemoryBus(type => CreateInstance(type, serviceProvider));
                RegisterHandlers(eventDispatcher);
                return eventDispatcher;
            });
            services.AddSingleton<IEventStore, InMemoryEventStore>();
        }

        private static void RegisterHandlers(IEventDispatcher eventDispatcher)
        {
            eventDispatcher.RegisterHandler<NewsItemDenormalizer>();
            eventDispatcher.RegisterHandler<ForumThreadDenormalizer>();
        }

        private static object CreateInstance(Type type, IServiceProvider serviceProvider)
        {
            var ctor = type.GetConstructors()
                .Where(c => c.IsPublic)
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault()
                ?? throw new InvalidOperationException($"No suitable contructor found on type '{type}'");

            var injectionServices = ctor.GetParameters()
                .Select(p => serviceProvider.GetRequiredService(p.ParameterType))
                .ToArray();

            return ctor.Invoke(injectionServices);
        }
    }
}

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
using Rebus.Logging;
using Microsoft.EntityFrameworkCore;
using DevPortal.CommandStack.Events;
using Rebus.Config;
using DevPortal.QueryStack;
using Microsoft.AspNetCore.Builder;
using Rebus.Extensions;
using Microsoft.Extensions.Configuration;

namespace DevPortal.Web.AppCode.EventSourcing
{

    public static class EventSourcingConfig
    {
        private static IBus _bus;

        public static void UseRebusEventSourcing(this IApplicationBuilder app)
        {
            var rebusServices = new ServiceCollection();
            rebusServices.AutoRegisterHandlersFromAssemblyOf<ActivityDenormalizer>();
            rebusServices.AddLogging();
            rebusServices.AddSingleton<IEventStore>(sp => app.ApplicationServices.GetRequiredService<IEventStore>());
            rebusServices.AddTransient<DevPortalDbContext>(sp =>
            {
                var messageContext = MessageContext.Current
                  ?? throw new InvalidOperationException("MessageContext.Current is null.");

                return messageContext.TransactionContext.Items
                    .GetOrThrow<DevPortalDbContext>(nameof(DevPortalDbContext));
            });


            rebusServices.AddRebus((configure, sp) => configure
              .Logging(l => l.Trace())
              .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "Messages"))
              .Routing(r => r.TypeBased().MapAssemblyOf<DomainEvent>("Messages"))
              .Options(o =>
              {
                  o.EnableUnitOfWork<DevPortalDbContext>(
                      unitOfWorkFactoryMethod: messageContext =>
                      {
                          var scope = app.ApplicationServices.CreateScope();
                          messageContext.TransactionContext.Items["ServiceScope"] = scope;
                          var dbContext = ActivatorUtilities.CreateInstance<DevPortalDbContext>(scope.ServiceProvider);
                          messageContext.TransactionContext.Items[nameof(DevPortalDbContext)] = dbContext;
                          return dbContext;
                      },
                      commitAction: (messageContext, dbContext) => dbContext.SaveChanges(),
                      cleanupAction: (messageContext, dbContext) => {

                          dbContext.Dispose();
                          var scope = (IServiceScope)messageContext.TransactionContext.Items["ServiceScope"];
                          scope.Dispose();
                      });

                  o.Decorate<IPipeline>(c =>
                  {
                      var pipeline = c.Get<IPipeline>();
                      var handlerNotifications = app.ApplicationServices.GetRequiredService<IHandlerNotifications>();
                      var step = new MyDispatchIncomingMessageStep(handlerNotifications, c.Get<IRebusLoggerFactory>());
                      return new PipelineStepInjector(pipeline).OnReceive(step, PipelineRelativePosition.Before, typeof(DispatchIncomingMessageStep));
                  });
                  o.Decorate<IPipeline>(c =>
                  {
                      var pipeline = c.Get<IPipeline>();
                      return new PipelineStepRemover(pipeline).RemoveIncomingStep(s => s.GetType() == typeof(DispatchIncomingMessageStep));
                  });
              }));

            var rebusServiceProvider = rebusServices.BuildServiceProvider();
            rebusServiceProvider.UseRebus();
            _bus = rebusServiceProvider.GetRequiredService<IBus>();
        }

        public static void AddSqlEventSourcing(this IServiceCollection services)
        {
            services.AddSingleton<IEventDispatcher, RebusEventDispatcher>();
            services.AddSingleton<IEventStore, SqlEventStore>(sp => ActivatorUtilities.CreateInstance<SqlEventStore>(sp.CreateScope().ServiceProvider, new object[] { AllEvents.ToArray() }));
            //services.AddSingleton<IEventStore, InMemoryEventStore>();

            services.AddSingleton<IHandlerNotifications, HandlerNotififications>(sp =>
            {
                var handlerNotification = ActivatorUtilities.CreateInstance<HandlerNotififications>(sp);

                //make handler notification accessible from extension methods - needs a better solution
                EventStoreExtensions.HandlerNotificationAccessor = () => handlerNotification;
                return handlerNotification;
            });
            services.AddTransient<IBus>(sp => _bus);
        }

        public static IEnumerable<Type> AllDenormalizers => Assembly.GetAssembly(typeof(NewsItemDenormalizer))
            .GetExportedTypes()
            .Where(t => t.Name.EndsWith("Denormalizer"));

        public static IEnumerable<Type> AllEvents => Assembly.GetAssembly(typeof(NewsItemCreated))
            .GetExportedTypes()
            .Where(e => typeof(DomainEvent).IsAssignableFrom(e));
    }
}

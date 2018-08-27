using DevPortal.CommandStack.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevPortal.Web.AppCode.EventSourcing
{
    public static class EventStoreExtensions
    {
        public static Func<IHandlerNotifications> HandlerNotificationAccessor { get; set; }

        public static async Task<bool> SaveAndWaitForHandler(this IEventStore eventStore, DomainEvent evt, Type handler, int timeoutMiliseconds = 5000)
        {
            using (var listener = HandlerNotificationAccessor().BeginListen(handler, evt.Id))
            {
                eventStore.Save(evt);
                try
                {
                    await listener.Wait(new CancellationTokenSource(timeoutMiliseconds).Token);
                }
                catch(TaskCanceledException)
                {
                    return false;
                }
                return true;
            }
        }
    }
}

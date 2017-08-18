using System;

namespace DevPortal.CommandStack.Infrastructure
{
    public interface IEventDispatcher
    {
        void RegisterHandler(Type t);
        void Dispatch(DomainEvent @event);
    }

    public static class EventDispatcherExtentsions
    {
        public static void RegisterHandler<THandler>(this IEventDispatcher eventDispatcher)
        {
            eventDispatcher.RegisterHandler(typeof(THandler));
        }
    }
}
namespace DevPortal.CommandStack.Infrastructure
{
    public interface IEventDispatcher
    {
        void Dispatch(DomainEvent @event);
    }
}
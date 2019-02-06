using System;
using System.Threading.Tasks;

namespace DevPortal.CommandStack.Infrastructure
{
    public interface IEventDispatcher
    {
        Task Dispatch(DomainEvent @event);
    }
}
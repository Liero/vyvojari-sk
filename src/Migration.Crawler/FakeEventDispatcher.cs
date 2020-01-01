using DevPortal.CommandStack.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Migration.Crawler
{
    class FakeEventDispatcher : IEventDispatcher
    {
        public Task Dispatch(DomainEvent @event)
        {

            return Task.CompletedTask;
        }
    }
}

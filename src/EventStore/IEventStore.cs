using System;
using System.Threading.Tasks;

namespace EventStore
{
    public interface IEventStore
    {
        Task<IEvent<TData>> Save<TData>(TData evt);
        IObservable<IEvent<object>> SavedEvents { get; }
    }

}

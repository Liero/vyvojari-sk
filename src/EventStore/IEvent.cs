using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;

namespace EventStore
{
    public interface IEvent<out TData>
    {
        Guid Id { get; }
        DateTime Timestamp { get; }
        string EventType { get; }
        TData Data { get; }
    }

    internal class Event<TData>: IEvent<TData>
    {
        public Guid Id { get; internal set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        //[JsonConverter(typeof(UnixDateTimeConverter))] //this converts seconds, we need miliseconds.
        public DateTime Timestamp { get; internal set; }
        public string EventType { get; internal set; }
        public TData Data { get; internal set; }
    }

}

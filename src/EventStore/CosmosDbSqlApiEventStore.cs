using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace EventStore
{
    public class CosmosDbSqlApiEventStore : IEventStore
    {
        internal const string CalculateTimestampTriggerName = "CalculateTimestamp";

        private readonly Uri _collectionUri;
        private readonly DocumentClient _client;
        private RequestOptions _requestOptions;

        protected Subject<IEvent<object>> SavedEventsSubject { get; }

        public CosmosDbSqlApiEventStore(DocumentClient client, string databaseName, string eventsCollectionName = "Events")
        {
            _collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, eventsCollectionName);
            _client = client;
            _requestOptions = new RequestOptions
            {
                PreTriggerInclude = new List<string> { "CalculateTimestamp" },
            };
            SavedEventsSubject = new Subject<IEvent<object>>();
        }

        public IObservable<IEvent<object>> SavedEvents => SavedEventsSubject;


        /// <summary>
        /// Saves eventData and return EventDocument including server calculated timestamp
        /// </summary>
        public async Task<IEvent<TData>> Save<TData>(TData eventData)
        {
            if (eventData == null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

            var eventDocument = new Event<TData>
            {
                Id = Guid.NewGuid(), //todo: consider using eventData.Id....
                Data = eventData,
                EventType = ResolveTypeName(eventData.GetType())
            };

            var createResult = await _client.CreateDocumentAsync(_collectionUri, eventDocument, _requestOptions);
            long timestamp = createResult.Resource.GetPropertyValue<long>("Timestamp");
            eventDocument.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime;

            return eventDocument;
        }

        protected string ResolveTypeName(Type type) => type.Name;
    }

}

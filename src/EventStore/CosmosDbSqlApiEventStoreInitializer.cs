using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventStore
{
    public class CosmosDbSqlApiEventStoreInitializer
    {
        private readonly DocumentClient _client;
        const string CalculateTimestampTriggerName = "CalculateTimestamp";

        public CosmosDbSqlApiEventStoreInitializer(DocumentClient documentClient)
        {
            _client = documentClient;
        }

        public async Task CreateDatabaseAndCollectionIfNotExists(string databaseName = "EventStore", string eventsCollectionName = "Events")
        {
            var databaseResult = await _client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = databaseName });

            var collectionResult = await _client.CreateDocumentCollectionIfNotExistsAsync(
                databaseResult.Resource.SelfLink,
                new DocumentCollection { Id = eventsCollectionName });

            var eventCollectionUri = collectionResult.Resource.SelfLink;

            var trigger = _client.CreateTriggerQuery(eventCollectionUri)
                .Where(t => t.Id == CalculateTimestampTriggerName).AsEnumerable()
                .FirstOrDefault();

            if (trigger == null)
            {
                trigger = new Trigger
                {
                    Id = CalculateTimestampTriggerName,
                    Body = ReadEmbeddedResourse("CalculateTimestamp.js"),
                    TriggerOperation = TriggerOperation.Create,
                    TriggerType = TriggerType.Pre
                };
                var triggerResponse = await _client.CreateTriggerAsync(eventCollectionUri, trigger);
            }
        }

        private static string ReadEmbeddedResourse(string relativePath)
        {
            var assembly = typeof(CosmosDbSqlApiEventStoreInitializer).Assembly;

            string resourceName = $"{assembly.GetName().Name}.{relativePath.Replace('\\', '.')}";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    var exception = new ArgumentException($"Resource not found", nameof(relativePath));
                    exception.Data.Add("RelativePath", relativePath);
                    throw exception;
                };
                return new StreamReader(stream).ReadToEnd();
            }
        }
    }

}

using Microsoft.Azure.Documents.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EventStore.Tests
{

    [TestClass]
    public class EventStoreIntergationTests
    {
        private const string EndpointUri = "https://liero-cosmosdb-sql.documents.azure.com:443/";
        private const string PrimaryKey = "3qL9NMlu0Zl2jjM5qnxTOGI6jCIwGIaSs3N9aM7tdRVDeQ9zhL476vHwV4pa3ZEuRwfWHFgwRFuaGNKrMv1LAQ==";


        [TestMethod]
        public async Task CanCreateEventStoreDatabase()
        {
            using (var client = new DocumentClient(new Uri(EndpointUri), PrimaryKey))
            {
                var eventStoreInitializer = new CosmosDbSqlApiEventStoreInitializer(client);
                string tempDatabaseName = $"EventStore_{Guid.NewGuid()}";

                try
                {
                    //action
                    await eventStoreInitializer.CreateDatabaseAndCollectionIfNotExists(tempDatabaseName);
                }
                finally
                {
                    // cleanup
                    try
                    {
                        await client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(tempDatabaseName));
                    }
                    catch(Exception deleteException)
                    {
                        Trace.WriteLine(deleteException);
                    }
                }                
            }
        }

        [TestMethod]
        public async Task CanSaveDocumentStoreDatabase()
        {
            using (var client = new DocumentClient(new Uri(EndpointUri), PrimaryKey))
            {
                var eventStoreInitializer = new CosmosDbSqlApiEventStoreInitializer(client);
                string tempDatabaseName = $"EventStore_IntegrationTests";
                await eventStoreInitializer.CreateDatabaseAndCollectionIfNotExists(tempDatabaseName);

                var eventStore = new CosmosDbSqlApiEventStore(client, tempDatabaseName);

                var eventData = new TestEventData(Guid.NewGuid(), "tetat", new[] { "abc", "Def" });
                var evt = await eventStore.Save(eventData);

                Assert.AreEqual(evt.EventType, eventData.GetType().Name);
                Assert.AreNotEqual(evt.Id, default(Guid));
                Assert.AreNotEqual(evt.Timestamp, default(DateTime));
                await client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(tempDatabaseName));
            }
        }
    }


    class TestEventData
    {
        public TestEventData(Guid contentId, string title, string[] tags)
        {
            ContentId = contentId;
            Title = title;
            Tags = tags;
        }

        public Guid ContentId { get; }
        public string Title { get; }
        public string[] Tags { get; }
    }
}

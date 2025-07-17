using InventoryService.Persistance.Documents;
using MongoDB.Driver;

namespace InventoryService.Persistance.Infrastructure
{
    internal class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<OrderDocumentEntity> Orders =>
            _database.GetCollection<OrderDocumentEntity>("OrderLogs");
    }
}

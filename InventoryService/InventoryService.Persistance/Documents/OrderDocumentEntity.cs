using InventoryService.Business.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InventoryService.Persistance.Documents
{
    public class OrderDocumentEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("orderId")]
        public int OrderId { get; set; }

        [BsonElement("productName")]
        public string ProductName { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("movementType")]
        [BsonRepresentation(BsonType.String)]
        public MovementType MovementType { get; set; }

        [BsonElement("metadata")]
        public Dictionary<string, object> Metadata { get; set; } // Customer info, reason, etc.
    }
}

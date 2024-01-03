using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStore.Models
{
    public class Book
    {
        public const string Collection = "Books";
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Pages { get; set; }
        // serialize dates into readable human format
        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructuur.Entities
{
    public class Blog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public User Author { get; set; }
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
        public string Content { get; set; }
    }
}

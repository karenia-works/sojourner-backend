using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace back.Models
{
    public class House
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id{ get; set;}

        public bool longAvailable { get; set;}
    }
}
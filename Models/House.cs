using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace back.Models
{
    public class House
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id{ get; set;}
        public string name { get; set;}
        public string discribe{ get; set;}
        public string houseType { get; set;}
        public bool longAvailable { get; set;}
    }
}
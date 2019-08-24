using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace back.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id{ get; set;}
        public string hId { get; set;}
        public string uId { get; set;}
        public string startDate { get; set;}
        public string endDate { get; set;}
        public bool isLongRent { get; set;}


    }
}
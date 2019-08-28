using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sojourner.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string houseId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string userId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool isLongRent { get; set; }
        public bool isFinished { get; set; }
    }
}

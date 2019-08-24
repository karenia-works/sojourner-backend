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
        public string hId { get; set; }
        public string uId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool isLongRent { get; set; }


    }
}

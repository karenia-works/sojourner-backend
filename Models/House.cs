using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Sojourner.Models
{
    public class House
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string description { get; set; }//mk?
        public bool[] equipJudge { get; set; }//8个
        public string houseType { get; set; }
        public bool longAvailable { get; set; }
        public bool shortAvailbale { get; set; }
        public int longPrice { get; set; }//单价
        public int shortPrice { get; set; }//单价
        public bool[] noticeJudge { get; set; }//5个
    }
}

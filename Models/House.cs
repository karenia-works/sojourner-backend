using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
namespace Sojourner.Models
{

    public class Address
    {
        public string country { get; set; };
        public string city { get; set; };
        public string street { get; set; };
    }

    public class House
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string name { get; set; }
        public Address address { get; set; }
        public string description { get; set; }//mk?
        public bool[] equipJudge { get; set; }//8个
        public string type { get; set; }//124
        public bool longAvailable { get; set; }
        public bool shortAvailable { get; set; }
        public int longPrice { get; set; }//单价
        public int shortPrice { get; set; }//单价
        public bool[] noticeJudge { get; set; }//5个
        public string[] img { get; set; }//房屋所有图片
    }
}

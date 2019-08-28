using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sojourner.Models
{
    public class Issue
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string wid { get; set; }//维修师傅
        public string[] img { get; set; }
        public string complaint { get; set; }
        public bool needRepair { get; set; }//是否需要报修
        public string reply{get;set;}
        public bool isReplied{get;set;}
    }
}
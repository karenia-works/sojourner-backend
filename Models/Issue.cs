using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sojourner.Models
{
    public class Issue
    {
        [BsonId]//
        [BsonRepresentation(BsonType.ObjectId)]//外键也要加
        public string id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string uid { get; set; }//用户
        [BsonRepresentation(BsonType.ObjectId)]
        public string wid { get; set; }//维修师傅
        [BsonRepresentation(BsonType.ObjectId)]
        public string hid { get; set; }//房屋
        [BsonRepresentation(BsonType.ObjectId)]
        public string[] img { get; set; }
        public string complaint { get; set; }
        public bool needRepair { get; set; }//是否需要报修
        public string reply{get;set;}
        public bool isReplied{get;set;}
        public DateTime createTime{get;set;}//报修单创建时间
    }
}
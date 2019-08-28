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
        public string houseId { get; set; }
        public string userId { get; set; }
        public DateTime startDate { get; set; }//入住时间
        public DateTime endDate { get; set; }//退住时间
        public bool isLongRent { get; set; }
        public bool isFinished { get; set; }

        //add
        public int totalPrice { get; set; }
        public DateTime createDate { get; set; }//订单创建时间
        public DateTime cancelDate { get; set; }//订单（自动）关闭时间
        public DateTime ddlDate { get; set; }//本轮月租支付截至时间
        public bool isPaid{get;set;}//本轮月租是否支付
        

    }
}

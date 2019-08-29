using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Text;

namespace Sojourner.Models
{
    public class Profile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string userId{ get; set;}
        public string sex{ get; set;}//sex: M(ale)/F(emale)/U(nknown)
        public DateTime signupDate{ get; set;}
        public string userName{get; set;}//username = nickname
        public string email{get; set;}
        
        //public string motto{get; set;}

        public string phoneNumber{get; set;}
        public string avatar{get; set;}

        public string role{get; set;}//worker admin IdentityServerApi
    }
}

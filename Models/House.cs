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
        public string description { get; set; }
        public string houseType { get; set; }
        public bool longAvailable { get; set; }
    }

    public class OrderedHouse : House
    {
        public OrderedHouse(House house, IEnumerable<Order> orders)
        {
            this.id = house.id;
            this.name = house.name;
            this.description = house.description;
            this.houseType = house.houseType;
            this.longAvailable = house.longAvailable;
            this.orders = orders;
        }
        public IEnumerable<Order> orders { get; set; }
    }
}

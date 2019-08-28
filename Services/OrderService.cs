using System;
using Sojourner.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using Sojourner.Models.Settings;
namespace Sojourner.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _orders;
        public OrderService(IDbSettings settings)
        {
            var client = new MongoClient(settings.DbConnection);
            var database = client.GetDatabase(settings.DbName);
            _orders = database.GetCollection<Order>(settings.OrderCollectionName);
        }
        public List<Order> findUserOrder(string uid)
        {
            var query = _orders.AsQueryable().
            Where(o => o.userId == uid).
            Select(o => o);
            return query.ToList();
        }
        public List<Order> findHouseOrder(string hid)
        {
            var query = _orders.AsQueryable().
            Where(o => o.houseId == hid).
            Select(o => o);
            return query.ToList();
        }
        public Order getOrderById(string oid)
        {
            var query = _orders.AsQueryable().
            Where(o => o.id == oid).First();
            return query;
        }
        public bool insertOrder(Order tar)
        {
            _orders.InsertOne(tar);
            return true;
        }
        public DeleteResult deleteOrder(Order tar)
        {
            var res = _orders.DeleteOne(o => o.id == tar.id);
            return res;
        }

        //修改isFinished
        public UpdateResult isFinishedChange(string oid)
        {
            var flicker = Builders<Order>.Filter.Eq("id",oid);
            var update = Builders<Order>.Update.Set("isFinished",true);
            var res = _orders.UpdateOne(flicker,update);

            return res;
        }

        // public DateTime calCancelDate(DateTime ct)
        // {
            
        // }
    }
}

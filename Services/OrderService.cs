using back.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using Sojourner.Models.Settings;
namespace back.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _orders;
        public OrderService(IDbSettings settings){
            var client =new MongoClient(settings.DbConnection);
            var database=client.GetDatabase(settings.DbName);
            _orders=database.GetCollection<Order>(settings.OrderCollectionName);
        }
        public bool insertOrder(Order tar){
            _orders.InsertOne(tar);
            return true;
        }
        public List<Order> findUserOrder(string uid){
            var query=_orders.AsQueryable().
            Where(o=>o.uId==uid).
            Select(o=>o);
            return query.ToList();
        } 
    }
}
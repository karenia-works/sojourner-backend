using Sojourner.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using Sojourner.Models.Settings;
using System.Threading.Tasks;
using System;
namespace Sojourner.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<Order> _finishedOrders;
        public OrderService(IDbSettings settings)
        {
            var client = new MongoClient(settings.DbConnection);
            var database = client.GetDatabase(settings.DbName);
            _orders = database.GetCollection<Order>(settings.OrderCollectionName);
            _finishedOrders = database.GetCollection<Order>(settings.finishedOrderCollectionName);
        }
        public List<Order> findUserOrder(string uid)
        {
            var query = _orders.AsQueryable().
            Where(o => o.userId == uid).
            Select(o => o);
            var queryfin = _finishedOrders.AsQueryable().
            Where(o => o.userId == uid).Select(o => o);
            (query.ToList()).AddRange(queryfin.ToList());
            return query.ToList();
        }
        public List<Order> findHouseOrder(string hid)
        {
            var query = _orders.AsQueryable().
            Where(o => o.houseId == hid).
            Select(o => o);
            var queryfin = _finishedOrders.AsQueryable().
            Where(o => o.houseId == hid).Select(o => o);
            (query.ToList()).AddRange(queryfin.ToList());
            return query.ToList();
        }
        public Order getOrderById(string oid)
        {
            var query = _orders.AsQueryable().
            Where(o => o.id == oid).Select(o => o);
            if (query.CountAsync().Result == 0)
                query = _finishedOrders.AsQueryable().Where(o => o.id == oid).Select(o => o);
            else
            {
                return query.First();
            }
            if (query.CountAsync().Result == 0)
                return null;
            else
                return query.First();

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
        async public Task<Order> isFinishedChange(string oid)
        {
            var query = _orders.AsQueryable().Where(o => o.id == oid).Select(o => o);
            if (query.CountAsync().Result == 0)
                return null;
            var tar = query.FirstAsync();
            await _finishedOrders.InsertOneAsync(tar.Result);
            return tar.Result;
        }
        async public Task<List<Order>> checkOrderDate(DateTime checkDate)
        {
            var query = _orders.AsQueryable().Where(order => order.endDate >= checkDate).Select(Order => Order);
            return await query.ToListAsync();
        }

    }
}

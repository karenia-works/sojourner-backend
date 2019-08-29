using System;
using Sojourner.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using Sojourner.Models.Settings;
using System.Threading.Tasks;
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
        public async Task<List<Order>> findUserActiveOrder(string uid)
        {
            var query = await _orders.AsQueryable()
                .Where(o => o.userId == uid).ToListAsync();
            return query;
        }

        public async Task<List<Order>> findUserFinishedOrder(string uid)
        {
            var query = await _finishedOrders.AsQueryable()
                .Where(o => o.userId == uid).ToListAsync();
            return query;
        }

        public async Task<List<Order>> findHouseOrder(string hid)
        {
            var query = _orders.AsQueryable()
                .Where(o => o.houseId == hid);
            var queryfin = _finishedOrders.AsQueryable()
                .Where(o => o.houseId == hid);
            var orders = (await query.ToListAsync());
            orders.AddRange(await queryfin.ToListAsync());
            return orders;
        }
        public async Task<Order> getOrderById(string oid)
        {
            var query = _orders.AsQueryable().
            Where(o => o.id == oid).Select(o => o);
            if (query.CountAsync().Result == 0)
                query = _finishedOrders.AsQueryable().Where(o => o.id == oid).Select(o => o);
            else
            {
                return await query.FirstOrDefaultAsync();
            }
            if (await query.CountAsync() == 0)
                return null;
            else
                return await query.FirstOrDefaultAsync();

        }
        public async ValueTask<bool> insertOrder(Order tar)
        {
            await _orders.InsertOneAsync(tar);
            return true;
        }
        public async Task<DeleteResult> deleteOrder(string id)
        {
            var res = await _orders.DeleteOneAsync(o => o.id == id);
            return res;
        }

        //修改isFinished
        public async Task<Order> isFinishedChange(string oid)
        {
            var query = _orders.AsQueryable().Where(o => o.id == oid).Select(o => o);
            if (query.CountAsync().Result == 0)
                return null;
            var tar = await query.FirstAsync();
            _orders.DeleteOne(Order => Order.id == tar.id);
            await _finishedOrders.InsertOneAsync(tar);
            return tar;
        }

        // public DateTime calCancelDate(DateTime ct)
        // {

        // }
        async public Task<List<Order>> checkOrderDate(DateTime checkDate)
        {
            var query = _orders.AsQueryable().Where(order => order.endDate >= checkDate).Select(Order => Order);
            return await query.ToListAsync();
        }

    }
}

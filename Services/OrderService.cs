using System;
using Sojourner.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using Sojourner.Models.Settings;
using System.Threading.Tasks;
using MongoDB.Bson;

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
                .Where(o => o.userEmail == uid).ToListAsync();
            return query;
        }

        public async Task<List<Order>> findUserFinishedOrder(string uid)
        {
            var query = await _finishedOrders.AsQueryable()
                .Where(o => o.userEmail == uid).ToListAsync();
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
        public async Task<List<Order>> getAllOrder()
        {
            var query = await _orders.AsQueryable().ToListAsync();
            return query;
        }
        public async ValueTask<bool> insertOrder(Order tar)
        {
            //可能有问题 太迟了查了也查不到
            tar.endDate = tar.startDate.AddDays(7);
            tar.isFinished = false;
            tar.cancelDate = tar.createDate.AddDays(7);
            tar.ddlDate = tar.startDate.AddMonths(1);
            tar.isPaid = false;
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

        public class ExtendedOrder : Order
        {
            public House house { get; set; }
        }

        async public Task<List<ExtendedOrder>> getAdminOrderPage(string kw)
        {
            var queryDefinition = new BsonDocument[]
            {
                new BsonDocument("$lookup",
                new BsonDocument
                    {
                        { "from", "houses" },
                        { "localField", "houseId" },
                        { "foreignField", "_id" },
                        { "as", "house" }
                    }),
                new BsonDocument("$unwind",
                new BsonDocument
                    {
                        { "path", "$house" },
                        { "preserveNullAndEmptyArrays", true }
                    }),
                new BsonDocument("$match",
                new BsonDocument
                    {
                        {"house.name", new BsonRegularExpression(kw, "i")}
                    }
                )
            };

            var ordersView = await _orders.AggregateAsync(
                PipelineDefinition<Order, ExtendedOrder>
                .Create(
                    queryDefinition
                )
            );


            List<ExtendedOrder> list = await ordersView.ToListAsync();
            return list;
        }

        public async Task<UpdateResult> extendOrderDate(string oid, DateTime time)
        {
            var flicker = Builders<Order>.Filter.Eq("id", oid);
            var update = Builders<Order>.Update.Set("endDate", time);
            var res = await _orders.UpdateOneAsync(flicker, update);

            return res;

        }
        public async Task<List<Order>> getOrderList()
        {
            var query = await _orders.AsQueryable().ToListAsync();
            return query;
        }
        async public Task<List<ExtendedOrder>> getUserOrderPage(string email, string kw)
        {
            var queryDefinition = new BsonDocument[]
            {
                new BsonDocument("$lookup",
                new BsonDocument
                    {
                        { "from", "houses" },
                        { "localField", "houseId" },
                        { "foreignField", "_id" },
                        { "as", "house" }
                    }),
                new BsonDocument("$unwind",
                new BsonDocument
                    {
                        { "path", "$house" },
                        { "preserveNullAndEmptyArrays", true }
                    }),
                new BsonDocument("$match",
                new BsonDocument
                    {
                        {"house.name", new BsonRegularExpression(kw, "i")},
                        {"userEmail", email}
                    }
                )
            };

            var ordersView = await _orders.AggregateAsync(
                PipelineDefinition<Order, ExtendedOrder>
                .Create(
                    queryDefinition
                )
            );


            List<ExtendedOrder> list = await ordersView.ToListAsync();
            return list;
        }


    }
}

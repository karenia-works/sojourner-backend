using Sojourner.Models.Settings;
using Sojourner.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Sojourner.Services
{
    public class HousesService
    {
        private readonly IMongoCollection<House> _houses;
        private readonly IMongoDatabase database;
        private readonly IDbSettings settings;
        public HousesService(IDbSettings settings)
        {
            this.settings = settings;
            var client = new MongoClient(settings.DbConnection);
            database = client.GetDatabase(settings.DbName);
            _houses = database.GetCollection<House>(settings.HouseCollectionName);
        }
        public House getHouseId(String id)
        {
            var query = _houses.AsQueryable().
                Where(o => o.id == id).First();
            return query;
        }
        public List<House> takeAvailableLong()
        {
            var query = _houses.AsQueryable().
                        Where(h => h.longAvailable == true).
                        Select(h => h);

            return query.ToList();
        }
        public List<House> takeAvailableShort(string startDate, string endDate)
        {
            var sd = DateTime.Parse(startDate);
            var ed = DateTime.Parse(endDate);
            //change later
            var query = from o in database.GetCollection<Order>(settings.OrderCollectionName).AsQueryable()
                        where o.startDate < ed || o.endDate > sd
                        select o.hId;
            return _houses.Find(s => s.longAvailable == false && !query.Contains(s.id)).ToList<House>();
        }
        // public IMongoQueryable<House> takeAvailableAll(String startDate, String endDate)
        // {
        //     var sd = DateTime.Parse(startDate);
        //     var ed = DateTime.Parse(endDate);
        //     //change later
        //     var query = database.GetCollection<Order>(settings.OrderCollectionName).AsQueryable()
        //                 .Where(o => o.startDate < ed || o.endDate > sd).Join(database.GetCollection<settings.)
        //      _houses.Find(s => s.longAvailable || !query.Any(item => item == s.id));
        // }

        public IMongoQueryable<House> searchForHouse(
            DateTime startTime, DateTime endTime, IList<string> keywords, ISet<string> houseType,
            int limit, int skip
        )
        {


            return this._houses.AsQueryable().Where(house => houseType.Contains(house.houseType)).GroupJoin(
                database.GetCollection<Order>(settings.OrderCollectionName).AsQueryable(),
                house => house.id,
                (Order order) => order.hId,
                (house, orders) => new OrderedHouse(house, orders)
            ).Where(h => h.orders.Any()).Select(obj => (House)obj);
        }

        public bool insertHouse(House tar)
        {
            _houses.InsertOne(tar);
            return true;
        }
        public bool insertHouseMany(List<House> tars)
        {
            _houses.InsertMany(tars);
            return true;
        }
    }
}

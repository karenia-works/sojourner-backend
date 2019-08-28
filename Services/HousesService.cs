using System.Collections.Immutable;
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
        public House getHouseById(String id)
        {
            var query = _houses.AsQueryable().
                Where(o => o.id == id).FirstOrDefault();
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
                        select o.houseId;
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

        public Task<List<House>> searchForHouse(
            DateTime startTime, DateTime endTime, IEnumerable<string> keywords, IEnumerable<string> houseType,
            int limit, int skip
        )
        {
            const string _ordersName = "orders";
            var aggregate = _houses.Aggregate(
                PipelineDefinition<House, House>.Create(
                    new BsonDocument[]
                    {
                        new BsonDocument("$match", new BsonDocument(
                            new List<BsonElement>(){
                                new BsonElement("name",
                                    new BsonRegularExpression(string.Join('|', keywords), "i")
                                ),
                                new BsonElement("longAvailable", true),
                                new BsonElement("houseType",
                                    new BsonDocument("$in", new BsonArray(houseType))
                                )
                            }
                        )),
                        new BsonDocument("$lookup",
                            new BsonDocument(
                            new List<BsonElement>(){
                                new BsonElement("from", settings.OrderCollectionName),
                                new BsonElement("localField", "_id"),
                                new BsonElement("foreignField", "houseId"),
                                new BsonElement("as", _ordersName)
                            })
                        ),
                        new BsonDocument(
                            "$match",
                            new BsonDocument(_ordersName,
                                new BsonDocument(
                                    "$not",
                                    new BsonDocument("$elemMatch",
                                        new BsonDocument(
                                        new List<BsonElement>{
                                            new BsonElement("startDate", new BsonDocument("$lt", endTime)),
                                            new BsonElement("endDate", new BsonDocument("$gt", startTime))
                                        }
                                        )
                                    )
                                )
                            )
                        ),
                        new BsonDocument("$project", new BsonDocument("orders", false)),
                        new BsonDocument("$skip", skip),
                        new BsonDocument("$limit", limit)
                    }
                )
            );

            return aggregate.ToListAsync();
        }

        public void insertHouse(House tar)
        {
            _houses.InsertOne(tar);
        }
        public Task insertHouseManyAsync(List<House> tars)
        {
            return _houses.InsertManyAsync(tars);
        }

        public DeleteResult deleteHouse(House tar)
        {
            var res = _houses.DeleteOne(o => o.id == tar.id);
            return res;
        }

        public ReplaceOneResult updateHouse(House tar)
        {
            var res = _houses.ReplaceOne(o => o.id == tar.id, tar);
            return res;
        }


    }
}

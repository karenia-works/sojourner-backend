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
        public async Task<House> getHouseById(String id)
        {
            var query = await _houses.AsQueryable().
                Where(o => o.id == id).FirstOrDefaultAsync();
            return query;
        }
        public async Task<List<House>> takeAvailableLong()
        {
            var query = await _houses.AsQueryable().
                        Where(h => h.longAvailable == true).
                        ToListAsync();
            return query;
        }

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
                                new BsonElement("type",
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

        public async Task insertHouse(House tar)
        {
            await _houses.InsertOneAsync(tar);
        }
        public Task insertHouseManyAsync(List<House> tars)
        {
            return _houses.InsertManyAsync(tars);
        }

        public async Task<DeleteResult> deleteHouse(string id)
        {
            var res = await _houses.DeleteOneAsync(o => o.id == id);
            return res;
        }

        public async Task<ReplaceOneResult> updateHouse(House tar)
        {
            var res = await _houses.ReplaceOneAsync(o => o.id == tar.id, tar);
            return res;
        }


    }
}

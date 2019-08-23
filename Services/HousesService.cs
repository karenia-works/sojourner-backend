using Sojourner.Models.Settings;
using back.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Collections.Generic;
using System;
namespace Sojourner.Services
{
    public class HousesService
    {
        private readonly IMongoCollection<House> _houses; 
        private readonly IMongoDatabase database;
        private readonly IDbSettings settings;
        public HousesService (IDbSettings settings){
            this.settings=settings;
            var client =new MongoClient(settings.DbConnection);
            database=client.GetDatabase(settings.DbName);
            _houses=database.GetCollection<House>(settings.HouseCollectionName);
        }

        public List<House> takeAvailableLong (){
            var query= _houses.AsQueryable().
                        Where(h=>h.longAvailable==true).
                        Select(h=>h);
                        
            return query.ToList();
        }
        public List<House> takeAvailableShort (string startdate,string enddate){
            int sd=Int32.Parse(startdate);
            int ed=Int32.Parse(enddate);
            //change later
            var query=from o in database.GetCollection<Order>(settings.OrderCollectionName).AsQueryable()
                    where Int32.Parse(o.startDate)<ed||Int32.Parse(o.endDate)>sd
                    select o.hId;
            return _houses.Find(s=>!query.Contains(s.id)).ToList<House>();
        }
        public bool insertHouse(House tar){
            _houses.InsertOne(tar);
            return true;
        }
        public bool insertHouseMany(List<House> tars){
            _houses.InsertMany(tars);
            return true;
        }
    }
}
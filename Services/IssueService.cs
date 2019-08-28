using System;
using Sojourner.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using Sojourner.Models.Settings;

namespace Sojourner.Services
{
    public class IssueService
    {
        private readonly IMongoCollection<Issue> _issues;
        public IssueService(IDbSettings settings)
        {
            var client = new MongoClient(settings.DbConnection);
            var database = client.GetDatabase(settings.DbName);
            _issues = database.GetCollection<Issue>(settings.IssueCollectionName);
        }
        public Issue getIssueById(string id)
        {
            var query = _issues.AsQueryable().
                Where(o => o.id == id).First();
            return query;
        }
        public UpdateResult replyToComplain(string iid,string reply)
        {
            var flicker = Builders<Issue>.Filter.Eq("id",iid);
            var update = Builders<Issue>.Update.Set("reply",reply).Set("isReplied",true);
            var res = _issues.UpdateOne(flicker,update);

            return res;
        }
        public UpdateResult setWorker(string iid,string wid)
        {
            var flicker = Builders<Issue>.Filter.Eq("id",iid);
            var update = Builders<Issue>.Update.Set("iid",iid).Set("wid",wid);
            var res = _issues.UpdateOne(flicker,update);

            return res;
        }
        
        
    }
}
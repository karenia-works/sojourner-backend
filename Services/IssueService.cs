using System;
using Sojourner.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using Sojourner.Models.Settings;
using System.Threading.Tasks;

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
        public async Task<Issue> getIssueById(string id)
        {
            var query = await _issues.AsQueryable().
                Where(o => o.id == id).FirstOrDefaultAsync();
            return query;
        }
        public async ValueTask<bool> insertIssue(Issue tar)
        {
            await _issues.InsertOneAsync(tar);
            return true;
        }
        public async Task<UpdateResult> replyToComplain(string iid, string reply)
        {
            var flicker = Builders<Issue>.Filter.Eq("id", iid);
            var update = Builders<Issue>.Update.Set("reply", reply).Set("isReplied", true);
            var res = await _issues.UpdateOneAsync(flicker, update);

            return res;
        }

        public async Task<UpdateResult> sendWorker(string iid, string wid)
        {
            var flicker = Builders<Issue>.Filter.Eq("id", iid);
            var update = Builders<Issue>.Update.Set("wid", wid);
            var res = await _issues.UpdateOneAsync(flicker, update);

            return res;
        }

    }
}
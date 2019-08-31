using System;
using Sojourner.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using Sojourner.Models.Settings;
using System.Threading.Tasks;



namespace Sojourner.Services
{
    public class IssueService
    {
        private readonly IMongoCollection<Issue> _issues;
        private readonly IMongoCollection<Profile> _profiles;
        public IssueService(IDbSettings settings)
        {
            var client = new MongoClient(settings.DbConnection);
            var database = client.GetDatabase(settings.DbName);
            _issues = database.GetCollection<Issue>(settings.IssueCollectionName);
            _profiles = database.GetCollection<Profile>(settings.ProfileCollectionName);
        }
        public async Task<Issue> getIssueById(string id)
        {
            var query = await _issues.AsQueryable().
                Where(o => o.id == id).FirstOrDefaultAsync();
            return query;
        }
        public async Task<List<Issue>> getIssueListByUid(string uemail)
        {
            Console.WriteLine("104");
            var query = await _issues.AsQueryable().
                Where(o => o.uemail == uemail).ToListAsync();
            Console.WriteLine("105");
            return query;
        }
        public async Task<List<Issue>> getIssueListByWid(string wemail)
        {
            var query = await _issues.AsQueryable().
                Where(o => o.wemail == wemail&&o.isFinished==false).ToListAsync();
            return query;
        }
        public async ValueTask<bool> insertIssue(Issue tar)
        {
            await _issues.InsertOneAsync(tar);
            return true;
        }
        public async Task<UpdateResult> replyToComplain(string iid, string reply, bool needRepair)
        {
            var flicker = Builders<Issue>.Filter.Eq("id", iid);
            var update = Builders<Issue>.Update.Set("reply", reply).Set("isReplied", true);
            if (needRepair != true)
                update = Builders<Issue>.Update.Set("isFinished", true);
            var res = await _issues.UpdateOneAsync(flicker, update);

            return res;
        }

        public async Task<UpdateResult> sendWorker(string iid, string wemail)
        {
            var flicker = Builders<Issue>.Filter.Eq("id", iid);
            var update = Builders<Issue>.Update.Set("wemail", wemail);
            var flicker2 = Builders<Profile>.Filter.Eq("email",wemail);
            var update2 = Builders<Profile>.Update.Set("isRenting",true);
            await _profiles.UpdateOneAsync(flicker2,update2); 
            var res = await _issues.UpdateOneAsync(flicker, update);

            return res;
        }
        public async Task<List<Issue>> getIssueList()
        {
            Console.WriteLine("252");
            var query = await _issues.AsQueryable().ToListAsync();
            Console.WriteLine("253");
            return query;
        }
        public async Task<List<Issue>> getUnFinishedIssueList()
        {
            var query = await _issues.AsQueryable().Where(Issue => Issue.isFinished == false).OrderByDescending(o => o.createTime).ToListAsync();
            return query;
        }
        public async Task<List<Issue>> getNeedRepairIssueList(string wemail)
        {
            Console.WriteLine("18");
            var query = await _issues.AsQueryable().
                Where(o => o.wemail == wemail && o.needRepair == true).ToListAsync();
            Console.WriteLine("17");
            return query;
        }
        public async Task<UpdateResult> confirmFinish(string wemail,string iid)
        {
            var flicker = Builders<Issue>.Filter.Eq("id", iid);
            var flicker2=Builders<Profile>.Filter.Eq("email",wemail);
            var update = Builders<Issue>.Update.Set("isFinished", true);
            var update2=Builders<Profile>.Update.Set("isRenting",false);
            await _profiles.UpdateOneAsync(flicker2,update2);
            var res = await _issues.UpdateOneAsync(flicker, update);

            return res;
        }

        public async Task<DeleteResult> deleteIssue(string id)
        {
            var res = await _issues.DeleteOneAsync(i => i.id == id);
            return res;
        }

        /*
        new BsonArray
{
    new BsonDocument("$lookup", 
    new BsonDocument
        {
            { "from", "houses" }, 
            { "localField", "hid" }, 
            { "foreignField", "_id" }, 
            { "as", "house" }
        }),
    new BsonDocument("$unwind", 
    new BsonDocument
        {
            { "path", "$house" }, 
            { "preserveNullAndEmptyArrays", true }
        })
}
 */
    }
}

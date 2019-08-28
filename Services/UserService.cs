using MongoDB.Driver;
using MongoDB.Bson;
using Sojourner.Models;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using Sojourner.Models.Settings;
using System.Threading.Tasks;
namespace Sojourner.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        public UserService(IDbSettings settings)
        {
            var client = new MongoClient(settings.DbConnection);
            var database = client.GetDatabase(settings.DbName);
            _users = database.GetCollection<User>(settings.UserCollectionName);
        }

        public User getUserId(string id)
        {
            var query = _users.AsQueryable().
                Where(u => u.id == id).First();
            return query;
        }

        public void insertUser(User tar)
        {
            _users.InsertOne(tar);
        }
        public async Task<User> findUser(string username, string password)
        {
            var query = await _users.AsQueryable().
            Where(user => user.username == username && user.password == password).ToListAsync();
            if (query.Count == 0)
            {
                return null;
            }
            return query[0];
        }

        public List<User> getWorker()
        {
            var query = _users.AsQueryable().
            Where(user => user.role =="worker").ToList();
            if (query.Count==0)
            {
                return null;
            }
            return query;
        }
        public List<User> findClearUserName(string keyword)
        {
            var query = _users.AsQueryable().
            Where(u => u.username == keyword).
            Select(u => u);
            return query.ToList();
        }

        public DeleteResult deleteUser(User tar)
        {
            var result = _users.DeleteOne(o => o.id == tar.id);
            return result;
        }

        public UpdateResult updateUser(User user)
        {
            var flicker = Builders<User>.Filter.Eq("id", user.id);
            var update = Builders<User>.Update.Set("username", user.username).Set("password", user.password).Set("role", user.role);
            var result = _users.UpdateOne(flicker, update);
            return result;
        }
    }
}

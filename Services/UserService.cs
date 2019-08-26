using MongoDB.Driver;
using MongoDB.Bson;
using Sojourner.Models;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using Sojourner.Models.Settings;
namespace Sojourner.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        public UserService(IDbSettings settings)
        {
            var client = new MongoClient(settings.DbConnection);
            var database = client.GetDatabase(settings.DbConnection);
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

        public UpdateResult editUser(User user)
        {
            var flicker = Builders<User>.Filter.Eq("id", user.id);
            var update = Builders<User>.Update.Set("username", user.username).Set("password", user.password).Set("state", user.state);
            var result = _users.UpdateOne(flicker, update);
            return result;
        }
    }
}

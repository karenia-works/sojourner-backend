using MongoDB.Driver;
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

        public bool insertUser(User tar)
        {
            _users.InsertOne(tar);
            return true;
        }
        public async Task<User> findUser(string username, string password)
        {
            var query = await _users.AsQueryable().
            Where(user => user.username == username && user.password == password).
            FirstAsync();
            return query;
        }
        public List<User> findClearUserName(string keyword)
        {
            var query = _users.AsQueryable().
            Where(u => u.username == keyword).
            Select(u => u);
            return query.ToList();
        }

        public bool removeUser(User tar)
        {
            _users.DeleteOne(o => o.id == tar.id);
            return true;
        }
    }
}

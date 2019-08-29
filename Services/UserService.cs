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

        public async Task<User> getUserId(string id)
        {
            var query = await _users.AsQueryable().
                Where(u => u.id == id).FirstOrDefaultAsync();
            return query;
        }

        public async Task insertUser(User tar)
        {
            // This work is done before insert, and also before any password updates
            tar.hashMyPassword();
            await _users.InsertOneAsync(tar);
        }

        public async ValueTask<bool> updatePassword(string username, string password)
        {
            var foundUser = await _users.AsQueryable()
                .Where(user => user.username == username).FirstOrDefaultAsync();
            if (foundUser == null)
            {
                return false;
            }
            else
            {
                foundUser.password = password;
                foundUser.hashMyPassword();

                var replaceResult = await _users.ReplaceOneAsync((user) => user.username == username, foundUser);

                return replaceResult.IsAcknowledged && replaceResult.ModifiedCount > 0;
            }
        }

        /// <summary>
        /// Find the user account matching specified username. If nothing matches,
        /// returns null.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<User> findUser(string username)
        {
            var query = await _users.AsQueryable().
            Where(user => user.username == username).FirstOrDefaultAsync();
            return query;
        }

        public async Task<List<User>> getWorker()
        {
            var query = _users.AsQueryable().
            Where(user => user.role == "worker").Select(getWorker=>getWorker);
            return await query.ToListAsync();
        }
        public async Task<User> findClearUserName(string keyword)
        {
            var query = await _users.AsQueryable().
            Where(u => u.username == keyword).FirstOrDefaultAsync();
            return query;
        }

        public async Task<DeleteResult> deleteUser(string id)
        {
            var result = await _users.DeleteOneAsync(o => o.id == id);
            return result;
        }

        public async Task<UpdateResult> updateUser(User user)
        {
            var flicker = Builders<User>.Filter.Eq("id", user.id);
            var update = Builders<User>.Update.Set("username", user.username).Set("password", user.password).Set("role", user.role);
            var result =await _users.UpdateOneAsync(flicker, update);
            return result;
        }
    }
}

using MongoDB.Driver;
using back.Models;
using MongoDB.Driver.Linq;
using Sojourner.Models;
namespace back.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        public UserService(){

        }
    }
}
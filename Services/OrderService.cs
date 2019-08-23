using back.Models;
using MongoDB.Driver;
using Sojourner.Models.Settings;
namespace back.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _orders;
        public OrderService(IDbSettings settings){
            var client =new MongoClient(settings.DbConnection);
            var database=client.GetDatabase(settings.DbName);
            _orders=database.GetCollection<Order>(settings.OrderCollectionName);
        }   
    }
}
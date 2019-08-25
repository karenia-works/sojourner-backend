using IdentityServer4.Models;
using System.Threading.Tasks;
using back.Interface;
using Sojourner.Models;
namespace back.Store
{
    public class ClientStore:IdentityServer4.Stores.IClientStore
    {
        public IRepository _dbRepository;
        public ClientStore(IRepository repository){
            _dbRepository=repository;
        }
        public Task<Client> FindClientByIdAsync(string ClientId){
            var client=_dbRepository.Single<Client>(c=>c.ClientId==ClientId);
            return Task.FromResult(client);
        }
    }
}
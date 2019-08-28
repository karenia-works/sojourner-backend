using System.Collections.Generic;
using IdentityServer4.Test;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using IdentityServer4;
using System.Security.Claims;
using Sojourner.Models.Settings;
namespace Sojourner.Store
{
    public class config
    {
        public static IConfiguration configuration { get; set; }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>{
                new Client{
                    ClientId="client",
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    ClientSecrets={
                        new Secret("client".Sha256())
                    },
                    AllowedScopes=new []{IdentityServer4.IdentityServerConstants.LocalApi.ScopeName},
                    // AllowedCorsOrigins=new[]{"*"}

                },

            };
        }


        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>{
                new ApiResource("worker","workerApi",new string[]{"Role"}),
                new ApiResource("admin","adminApi",new string[]{"Role"}),
                new ApiResource(IdentityServer4.IdentityServerConstants.LocalApi.ScopeName,"identityapi",new string[]{"Role"})
            };

        }
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>{
                new TestUser{
                    SubjectId="00001",
                    Username="liu",
                    Password="liu"
                }
            };
        }
    }


}

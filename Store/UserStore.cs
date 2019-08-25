using IdentityServer4.Validation;
using System.Threading.Tasks;
using Sojourner.Services;
namespace sojourner.Store
{
    public class UserStore:IResourceOwnerPasswordValidator
    {   
        public UserService _userservice {get;set;}
        public UserStore(UserService userService){
            _userservice=userService;
        }
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            
            throw new System.NotImplementedException();
        }   
    }
}
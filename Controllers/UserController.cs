using System;
using Microsoft.AspNetCore.Mvc;
using Sojourner.Models;
using Sojourner.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sojourner.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        //object type: string
        [HttpGet("/{id:regex([[0-9a-fA-F]]{{24}})}")]
        public User getUserId(string id)
        {
            var res = _userService.getUserId(id);
            if(res == null)
            {
                NotFound();
            }
            return res;
        }

        //object type: string
        [HttpDelete("/{id:regex([[0-9a-fA-F]]{{24}})}")]
        public string removeUser(string id)
        {
            var res = _userService.getUserId(id);
            if(res == null)
            {
                return "cannot found user";
            }
            else
            {
                var tem = _userService.removeUser(res);
                if(tem == false){
                    return "delete error";
                }
                else
                    return "delete successful";
            }
            
        }


        //object type: User
        [HttpPost("/{id:regex([[0-9a-fA-F]]{{24}})}")]
        public string addUser(User user_in)
        {
            var tem = _userService.findClearUserName(user_in.username);
            if(tem != null){
                return "user already exist";
            }
            else{
                var res = _userService.insertUser(user_in);
                if(res == false){
                    return "add user error";
                }
                return "add successful";
            }
            
        }

        
    }
}
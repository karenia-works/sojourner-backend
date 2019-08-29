using System;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
        [HttpGet("{id:regex([[0-9a-fA-F]]{{24}})}")]
        public async Task< User> getUserId(string id)
        {
            var res = await _userService.getUserId(id);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }

        [HttpGet("{username}")]
        public async Task<User> getUserByUserName(string userName){
            var res = await _userService.findClearUserName(userName);
            if(res == null){ 
                NotFound();
                //return StatusCode(StatusCodes.Status404NotFound, new {success = false, error = "user not exist"});
            }
            return res;
        }

        //object type: string
        [HttpDelete("{id:regex([[0-9a-fA-F]]{{24}})}")]
        public async Task<IActionResult> deleteUser(string id)
        {
            var res = await _userService.getUserId(id);
            if (res == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user not exist" });
            }
            else
            {
                var tem = await _userService.deleteUser(res);
                if (tem.DeletedCount != 1)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "delete error" });
                }
                else
                    return StatusCode(StatusCodes.Status200OK);
            }

        }


        //object type: User
        [HttpPost]
        public async Task<IActionResult> insertUser([FromBody] User user_in)
        {
            var tem = await _userService.findClearUserName(user_in.username);
            if (tem != null )
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user already exist" });
            }
            else
            {
                await _userService.insertUser(user_in);
                return StatusCode(StatusCodes.Status201Created);
            }
        }

        [HttpPost("{id:regex([[0-9a-fA-F]]{{24}})}")]
        public async Task<IActionResult> updateUser([FromBody]User user_in)
        {
            var res = await _userService.getUserId(user_in.id);
            if (res == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user not exist" });
            }
            UpdateResult result = await _userService.updateUser(user_in);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "edit error" });
            }
            else
                return StatusCode(StatusCodes.Status200OK);
        }

        [HttpGet("workers")]
        public async Task<List<User>> getWorkerList()
        {
            var result = await _userService.getWorker();
            if (result == null)
            {
                NotFound();
            }
            return result;
        }
    }
}

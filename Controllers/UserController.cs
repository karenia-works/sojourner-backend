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
        [HttpGet("/{id:regex([[0-9a-fA-F]]{{24}})}")]
        public User getUserId(string id)
        {
            var res = _userService.getUserId(id);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }

        //object type: string
        [HttpDelete("/{id:regex([[0-9a-fA-F]]{{24}})}")]
        public IActionResult deleteUser(string id)
        {
            var res = _userService.getUserId(id);
            if (res == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user not exist" });
            }
            else
            {
                var tem = _userService.deleteUser(res);
                if (tem.DeletedCount != 1)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "delete error" });
                }
                else
                    return StatusCode(StatusCodes.Status200OK);
            }

        }


        //object type: User
        [HttpPost("/{id:regex([[0-9a-fA-F]]{{24}})}")]
        public IActionResult insertUser(User user_in)
        {
            var tem = _userService.findClearUserName(user_in.username);
            if (tem != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user already exist" });
            }
            else
            {
                user_in.id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                _userService.insertUser(user_in);
                return StatusCode(StatusCodes.Status201Created);
            }

        }

        [HttpPost()]
        public IActionResult updateUser(User user_in)
        {
            var res = _userService.getUserId(user_in.id);
            if (res == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user not exist" });
            }
            UpdateResult result = _userService.updateUser(user_in);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "edit error" });
            }
            else
                return StatusCode(StatusCodes.Status200OK);
        }

        public List<User> getWorkerList()
        {
            var result = _userService.getWorker();
            if (result == null)
            {
                NotFound();
            }
            return result;
        }
    }
}

using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Sojourner.Models;
using Sojourner.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Sojourner.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class ProfileController : ControllerBase
    {
        private ProfileService _profileService;

        public ProfileController(ProfileService profileService){
            _profileService = profileService;
        }

        [HttpGet("{id:regex([[0-9a-fA-F]]{{24}})}")]
        public Profile getProfileById(string id){
            var res = _profileService.getProfileById(id);
            if(res == null){
                NotFound();
                //return StatusCode(StatusCodes.Status404NotFound, new {success = false, error = "user not exist"});
            }
            return res;
        }

        [HttpGet("{username}")]
        public Profile getProfileByUserName(string userName){
            var res = _profileService.getProfileByUserName(userName);
            if(res == null){ 
                NotFound();
                //return StatusCode(StatusCodes.Status404NotFound, new {success = false, error = "user not exist"});
            }
            return res;
        }

        [Authorize("adminApi")]
        public List<Profile> getProfileList(){
            var res = _profileService.getProfileList();
            return res;
        }

        [Authorize("adminApi")]
        [HttpDelete("/{id:regex([[0-9a-fA-F]]{{24}})}")]
        public IActionResult deleteProfile(string id)
        {
            var res = _profileService.getProfileById(id);
            if (res == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user profile not exist" });
            }
            else
            {
                var tem = _profileService.deleteProfile(res);
                if (tem.DeletedCount != 1)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user profile delete error" });
                }
                else
                    return StatusCode(StatusCodes.Status200OK);
            }
        }

        [Authorize("adminApi")]
        [HttpPost("{id:regex([[0-9a-fA-F]]{{24}})}")]
        public IActionResult insertProfile([FromBody] Profile user_in)
        {
            var tem = _profileService.getProfileByUserName(user_in.userName);
            if (tem != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user profile already exist" });
            }
            else
            {
                user_in.userId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                _profileService.insertProfile(user_in);
                return StatusCode(StatusCodes.Status201Created);
            }

        }

        [HttpPost("{id}")]
        public IActionResult updateProfile(string userId, [FromBody]Profile user_in)
        {
            if(userId != user_in.userId)
            {
                return BadRequest(new { success = false, error = "user id do not match" });
            }

            var res = _profileService.getProfileById(user_in.userId);
            
            if (res == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user profile not exist" });
            }
            UpdateResult result = _profileService.updateProfile(user_in);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "edit profile error" });
            }
            else
                return StatusCode(StatusCodes.Status200OK);
        }
    }
}

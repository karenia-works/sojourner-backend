using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Sojourner.Models;
using Sojourner.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

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
        public async Task<Profile> getProfileById(string id){
            var res = await _profileService.getProfileById(id);
            if(res == null){
                NotFound();
                //return StatusCode(StatusCodes.Status404NotFound, new {success = false, error = "user not exist"});
            }
            return res;
        }

        [HttpGet("{username}")]
        public async Task<Profile> getProfileByUserName(string userName){
            var res = await _profileService.getProfileByUserName(userName);
            if(res == null){ 
                NotFound();
                //return StatusCode(StatusCodes.Status404NotFound, new {success = false, error = "user not exist"});
            }
            return res;
        }

        [Authorize("adminApi")]
        public async Task<List<Profile>> getProfileList(){
            var res = await _profileService.getProfileList();
            return res;
        }

        [Authorize("adminApi")]
        [HttpDelete("/{id:regex([[0-9a-fA-F]]{{24}})}")]
        public async Task<IActionResult> deleteProfile(string id)
        {
            var tem = await _profileService.deleteProfile(id);
            if (tem.DeletedCount != 1)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user profile delete error" });
            }
            else
                return StatusCode(StatusCodes.Status200OK);
        }

        [Authorize("adminApi")]
        [HttpPost("{id:regex([[0-9a-fA-F]]{{24}})}")]
        public async Task<IActionResult> insertProfile([FromBody] Profile user_in)
        {
            var tem = await _profileService.getProfileByUserName(user_in.userName);
            if (tem != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user profile already exist" });
            }
            else
            {
                user_in.userId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                await _profileService.insertProfile(user_in);
                return StatusCode(StatusCodes.Status201Created);
            }

        }

        [HttpPost("{id}")]
        public async Task<IActionResult> updateProfile(string userId, [FromBody]Profile user_in)
        {
            if(userId != user_in.userId)
            {
                return BadRequest(new { success = false, error = "user id do not match" });
            }

            var res = await _profileService.getProfileById(user_in.userId);
            
            if (res == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user profile not exist" });
            }
            UpdateResult result = await _profileService.updateProfile(user_in);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "edit profile error" });
            }
            else
                return StatusCode(StatusCodes.Status200OK);
        }
    }
}

using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Sojourner.Models;
using Sojourner.Services;
using System.Collections.Generic;

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

        [HttpGet("/{id:regex([[0-9a-fA-F]]{{24}})}")]
        public Profile getProfileById(string id){
            var res = _profileService.getProfileById(id);
            if(res == null){
                NotFound();
                //return StatusCode(StatusCodes.Status404NotFound, new {success = false, error = "user not exist"});
            }
            return res;
        }

        [HttpGet("/{id:regex([[0-9a-fA-F]]{{24}})}")]
        public Profile getProfileByUserName(string userName){
            var res = _profileService.getProfileByUserName(userName);
            if(res == null){
                NotFound();
                //return StatusCode(StatusCodes.Status404NotFound, new {success = false, error = "user not exist"});
            }
            return res;
        }

        public List<Profile> getProfileList(){
            var res = _profileService.getProfileList();
            return res;
        }

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

        [HttpPost("/{id:regex([[0-9a-fA-F]]{{24}})}")]
        public IActionResult insertProfile(Profile user_in)
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

        [HttpPost()]
        public IActionResult updateProfile(Profile user_in)
        {
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
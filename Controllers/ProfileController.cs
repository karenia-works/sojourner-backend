using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Sojourner.Models;
using System.Security.Claims;
using System.Linq;
using Sojourner.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using IdentityServer4;

namespace Sojourner.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class ProfileController : ControllerBase
    {
        private ProfileService _profileService;

        public ProfileController(ProfileService profileService)
        {
            _profileService = profileService;
        }

        // [HttpGet("{id:regex([[0-9a-fA-F]]{{24}})}")]
        // public async Task<Profile> getProfileById(string id){
        //     var res = await _profileService.getProfileById(id);
        //     if(res == null){
        //         NotFound();
        //         //return StatusCode(StatusCodes.Status404NotFound, new {success = false, error = "user not exist"});
        //     }
        //     return res;
        // }
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpGet("me")]
        public async Task<Profile> getProfile(){
            var user=User.Claims.Where(Claim=>Claim.Type=="Name").FirstOrDefault().Value;
            var res=await _profileService.getProfileByEmail(user);
            if(res==null)
            {
                NotFound();
            }
            return res;
        }
        [Authorize("adminApi")]
        [HttpGet("{userName}")]
        public async Task<Profile> getProfileByUserName(string userName)
        {
            var res = await _profileService.getProfileByEmail(userName);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }

        // [Authorize("adminApi")]
        // [HttpGet("allUserList")]
        // public async Task<List<Profile>> getProfileList()
        // {
        //     var res = await _profileService.getProfileList();
        //     return res;
        // }

        // [Authorize("adminApi")]
        // [HttpGet("workerList")]
        // public async Task<List<Profile>> getWorkerProfileList()
        // {
        //     var res = await _profileService.getWorkerProfileList();
        //     return res;
        // }

        // [Authorize("adminApi")]
        // [HttpGet("userList")]
        // public async Task<List<Profile>> getUserProfileList()
        // {
        //     var res = await _profileService.getUserProfileList();
        //     return res;
        // }

        // [Authorize("adminApi")]
        // [HttpGet("adminList")]
        // public async Task<List<Profile>> getAdminProfileList()
        // {
        //     var res = await _profileService.getUserProfileList();
        //     return res;
        // }

        [Authorize("adminApi")]
        [HttpGet("allUserList/{role}")]
        public async Task<List<Profile>> getProfileList(string role)
        {
            if (role == "all")
            {
                var res = await _profileService.getProfileList();
                return res;
            }
            else
            {
                var res = await _profileService.getDirectProfileList(role);
                return res;
            }
        }

        [Authorize("adminApi")]
        [HttpDelete("{email}")]
        public async Task<IActionResult> deleteProfile(string email)
        {
            var tem = await _profileService.deleteProfile(email);
            if (tem.DeletedCount != 1)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user profile delete error" });
            }
            else
                return StatusCode(StatusCodes.Status200OK);
        }

        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPost]
        public async Task<IActionResult> insertProfile([FromBody] Profile user_in)
        {
            var tem = await _profileService.getProfileByEmail(user_in.email);
            if (tem != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user profile already exist" });
            }
            else
            {
                await _profileService.insertProfile(user_in);
                return StatusCode(StatusCodes.Status201Created);
            }
        }

        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPut()]
        public async Task<IActionResult> updateProfile([FromBody]Profile user_in)
        {
            var email = User.Claims.Where(claim => claim.Type == "sub").FirstOrDefault().Value;
            if (email != user_in.email)
            {
                return BadRequest(new { success = false, error = "user id do not match" });
            }

            var res = await _profileService.getProfileByEmail(user_in.email);

            if (res == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "user profile not exist" });
            }
            UpdateResult result = await _profileService.updateProfile(user_in);
            if (result.ModifiedCount == 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "edit profile error" });
            }
            else
                return StatusCode(StatusCodes.Status200OK);
        }
    }
}

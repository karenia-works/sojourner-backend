using System;
using Microsoft.AspNetCore.Mvc;
using Sojourner.Services;
using Sojourner.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IdentityServer4;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using MongoDB;
using Sojourner.Models.Settings;
using Sojourner.Store;
namespace Sojourner.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private HousesService _housesService;
        public RoomController(HousesService housesService)
        {
            _housesService = housesService;

        }

        [HttpGet("find")]
        public House getHouseById(string id = "5d612bfe2cc8473388248d5b")

        {
            var res = _housesService.getHouseById(id);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }

        [HttpGet()]
        public async Task<List<House>> searchHouses(string kw = "", string roomType = "single double quad", string startTime = "2099-1-1",
         string endTime = "2099-12-31", int limit = 20, int skip = 0)
        {
            var _startTime = DateTime.Parse(startTime);
            var _endTime = DateTime.Parse(endTime);
            if (roomType == null) roomType = "single double quad";
            var res = await _housesService.searchForHouse(_startTime, _endTime, kw.Split(' '), roomType.Split(' '), limit, skip);

            return res;
        }
        [Authorize("adminApi")]
        [HttpGet("insert")]

        public IActionResult insertHouse(House house)
        {
            house.id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            _housesService.insertHouse(house);
            return StatusCode(StatusCodes.Status201Created, new { id = house.id });
        }
        [Authorize("adminApi")]
        [HttpGet("delete")]

        public IActionResult deleteHouse(string hid)
        {
            var res = _housesService.getHouseById(hid);
            if (res == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "house not exist" });
            }
            else
            {
                var tem = _housesService.deleteHouse(res);
                if (tem.DeletedCount != 1)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "delete error" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
            }
        }
        [Authorize("adminApi")]
        public IActionResult updateHouse(House house)

        {
            if (house.id != null && house.id != id) return BadRequest();
            if (house.id == null) house.id = id;

            var res = _housesService.updateHouse(house);
            if (res != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "update error" });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK);
            }
        }
    }
}

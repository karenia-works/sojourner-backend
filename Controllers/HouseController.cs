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

        [HttpGet("{id:regex([[0-9a-fA-F]]{{24}})}")]
        public async Task<House> getHouseById(string id)
        {
            var res = await _housesService.getHouseById(id);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }

        [Authorize("adminApi")]
        [HttpGet("HouseList")]
        public async Task<List<House>> getHouseList()
        {
            var res = await _housesService.getHouseList();
            if (res == null)
                NotFound();
            return res;
        }

        [HttpGet()]
        public async Task<List<House>> searchHouses(
            string kw = "", string roomType = "single double quad", string startTime = "2099-1-1",
            string useLongRent = "null",
            string endTime = "2099-12-31", int limit = 20, int skip = 0)
        {
            var _startTime = DateTime.Parse(startTime);
            var _endTime = DateTime.Parse(endTime);

            if (kw == ":all") kw = "";

            bool includeLongRent, includeShortRent;

            switch (useLongRent)
            {
                case "true":
                    includeLongRent = true;
                    includeShortRent = false;
                    break;
                case "false":
                    includeLongRent = false;
                    includeShortRent = true;
                    break;
                default:
                    includeLongRent = true;
                    includeShortRent = true;
                    break;

            }

            if (roomType == null) roomType = "single double quad";
            var res = await _housesService.searchForHouse(
                _startTime,
                _endTime,
                keywords: kw.Split(' '),
                roomType: roomType.Split(' '),
                includeLongRent: includeLongRent,
                inclideShortRent: includeShortRent,
                limit,
                skip);

            return res;
        }

        [Authorize("adminApi")]
        [HttpPost]
        public async Task<IActionResult> insertHouse([FromBody]House house)
        {
            house.id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            await _housesService.insertHouse(house);
            return StatusCode(StatusCodes.Status201Created, new { id = house.id });
        }

        [Authorize("adminApi")]
        [HttpDelete("{id:regex([[0-9a-fA-F]]{{24}})}")]
        public async Task<IActionResult> deleteHouse(string id)
        {
            var tem = await _housesService.deleteHouse(id);
            if (tem.DeletedCount != 1)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "delete error" });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK);
            }
        }

        [Authorize("adminApi")]
        [HttpPut("{id:regex([[0-9a-fA-F]]{{24}})}")]
        public async Task<IActionResult> updateHouse(string id, [FromBody]House house)
        {
            if (await _housesService.getHouseById(house.id) == null)
                return StatusCode(StatusCodes.Status204NoContent);

            var res = await _housesService.updateHouse(house);
            if (res == null)
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

using System;
using Microsoft.AspNetCore.Mvc;
using Sojourner.Services;
using Sojourner.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using MongoDB;

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

        [HttpGet("/{id:regex([[0-9a-fA-F]]{{24}})}")]
        public House getHouseById(string id)
        {
            var res = _housesService.getHouseById(id);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }

        [HttpGet()]
        public async Task<List<House>> searchHouses(string kw = "", string room_type = "", string start_date = "2099-1-1",
         string end_date = "2099-12-31", int limit = 20, int skip = 0)
        {
            var startTime = DateTime.Parse(start_date);
            var endTime = DateTime.Parse(end_date);
            var res = await _housesService.searchForHouse(startTime, endTime, kw.Split(' '), room_type.Split(' '), limit, skip);

            return res;
        }

        [HttpGet("/insert")]
        public IActionResult insertHouse(House house)
        {
            house.id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            _housesService.insertHouse(house);
            return StatusCode(StatusCodes.Status201Created,new { id = house.id });
        }

        [HttpGet("/delete")]
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

        public IActionResult updateHouse(House house)
        {
            var res = _housesService.updateHouse(house);
            if(res != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,new{success = false,error = "update error"});
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK);
            }
        }



    }
}

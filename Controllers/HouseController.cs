using System;
using Microsoft.AspNetCore.Mvc;
using Sojourner.Services;
using Sojourner.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.Stores;
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

        [HttpGet("{id}")]
        public House getHouseId(string id)
        {
            var res = _housesService.getHouseId(id);
            // if (res == null)
            // {
            //     NotFound();
            // }
            return res;
        }
        // [HttpGet]
        // public List<House> showAvaliableLong()
        // {
        //     var res = _housesService.takeAvailableLong();
        //     return res;
        // }
        [HttpGet()]
        public async Task<List<House>> kwHouses(string kw = "", string room_type = "", string startTime = "2000-1-1",
         string endTime = "2099-12-31", int limit = 20, int skip = 0)
        {
            var start_Time = DateTime.Parse(startTime);
            var end_Time = DateTime.Parse(endTime);
            var res = await _housesService.searchForHouse(start_Time, end_Time, kw.Split(' '), room_type.Split(' '), limit, skip);

            return res;
        }

        [HttpGet("debug/insert")]
        public async Task<string> InsertHouses()
        {
            await _housesService.insertHouseManyAsync(
                new List<House>()
                {
                    new House(){
                        id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                        name = "Sample House 1",
                        description = "This is a sample house from a test database",
                        houseType = "single",
                        longAvailable = true,
                    },
                    new House(){
                        id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                        name = "Astronaut Beachhouse",
                        description = "This is a sample house from a test database",
                        houseType = "quad",
                        longAvailable = true,
                    },
                    new House(){
                        id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                        name = "International Space Station",
                        description = "This is a sample house from a test database",
                        houseType = "single",
                        longAvailable = true,
                    }
                }
            );
            return "Yay!";
        }
    }
}

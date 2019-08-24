using System;
using Microsoft.AspNetCore.Mvc;
using Sojourner.Services;
using Sojourner.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        [HttpGet("/{id}")]
        public House getHouseId(string id)
        {
            var res = _housesService.getHouseId(id);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }
        // [HttpGet]
        // public List<House> showAvaliableLong()
        // {
        //     var res = _housesService.takeAvailableLong();
        //     return res;
        // }
        [HttpGet()]
        public List<House> kwHouses(string kw = "", string room_type = "any", string start_date = "2000-1-1",
         string end_date = "2099-12-31", int limit = 20, int skip = 0)
        {
            var startTime = DateTime.Parse(start_date);
            var endTime = DateTime.Parse(end_date);
            var res = _housesService.searchForHouse(startTime, endTime, kw.Split(' '), new HashSet<string>() { room_type }, limit, skip);

            return res.ToList();
        }
    }
}

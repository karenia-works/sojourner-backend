using Microsoft.AspNetCore.Mvc;
using Sojourner.Services;
using back.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace back.Controllers
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
        public List<House> kwHouses(string kw = "", string room_type = "any", string start_date = "any",
         string end_date = "any", int limit = 20, int skip = 0)
        {
            var query_res = _housesService.takeAvailableAll(start_date, end_date);
            var match_res = room_type.Split('|');
            Regex regex = new Regex(@"(" + kw + @")+");
            var res = query_res.Where(h => match_res.Contains(h.houseType) && regex.IsMatch(h.name)).
            Take(limit).Skip(skip);
            return res.ToList();
        }
    }
}
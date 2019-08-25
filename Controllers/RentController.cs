using Microsoft.AspNetCore.Mvc;

using Sojourner.Models;
using Sojourner.Services;
namespace back.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RentController
    {
        private HousesService _housesService;
        private OrderService _orderService;
        public RentController(HousesService housesService, OrderService orderService){
            _housesService=housesService;
            _orderService=orderService;
        }
        /* [HttpPost]
        public string rent(RentRequest rentRequest){
            if(rentRequest.isLongRent){
                var order=new Order();
                order.isLongRent=true;
                order.startDate=rentRequest.startTime;
                order.endDate=rentRequest.endTime;
                order.uId=rentRequest.userId;
                order.hId=rentRequest.houseId;
               _orderService.insertOrder(order);
               return rentRequest.userId; 
            }
            else
                return "";
        }*/
    }
}
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Sojourner.Services;
using Sojourner.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sojourner.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class OrderController : ControllerBase
    {
        private OrderService _orderService;
        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("/{id}")]
        public Order findOrder(string id)
        {
            var res = _orderService.findOrder(id);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }

        // GET api/v1/order/for_user?uid=12345
        [HttpGet("/for_user")]
        public List<Order> findUserOrder(string uid)
        {
            var res = _orderService.findUserOrder(uid);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }

        [HttpGet("/for_house")]
        public List<Order> findHouseOrder(string hid)
        {
            var res = _orderService.findHouseOrder(hid);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }


        [HttpPost()]
        public IActionResult insertOrder(Order order)
        {
            var tem = _orderService.findOrder(order.id);
            if (tem != null)
            {
                //already exist
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "document already exist" });
            }
            else
            {
                var res = _orderService.insertOrder(order);
                if (res != false)
                {
                    //insert error
                    return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "insert error" });
                }
                else
                {
                    //insert successful
                    return StatusCode(StatusCodes.Status201Created);
                }

            }

        }
    }
}
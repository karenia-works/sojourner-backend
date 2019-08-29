using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Sojourner.Services;
using Sojourner.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4;
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

        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpGet("{id}")]
        public async Task<Order> findOrder(string id)

        {
            var res = await _orderService.getOrderById(id);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }

        // GET api/v1/order/for_user?uid=12345
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpGet("for_user")]
        public async Task<List<Order>> findUserOrder(string uid = "123451234512345123451234")
        {
            var res = await _orderService.findUserActiveOrder(uid);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }


        [HttpGet("for_house")]
        public async Task<List<Order>> findHouseOrder(string oid = "123451234512345123451234")

        {
            var res = await _orderService.findHouseOrder(oid);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }


        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpPost()]
        public async Task<IActionResult> insertOrder([FromBody]Order order)
        {
            order.id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            var res = await _orderService.insertOrder(order);
            if (res != false)
            {
                //insert error
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "insert error" });
            }
            else
            {
                //insert successful
                return StatusCode(StatusCodes.Status201Created, order.id);
            }
        }

        [Authorize("adminApi")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteOrder(string id)
        {
            var tem = await _orderService.deleteOrder(id);
            if (tem.DeletedCount != 1)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "delete error" });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK);
            }

        }

        [HttpGet("{oid}/setFinished")]
        public async Task<IActionResult> isFinishedChange(string oid)
        {
            var res = await _orderService.isFinishedChange(oid);
            if (res == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "update isFinished failed" });
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK);
            }
        }
    }
}

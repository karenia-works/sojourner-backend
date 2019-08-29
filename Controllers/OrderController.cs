using System.Net.Mime;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Sojourner.Services;
using Sojourner.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4;
using System.Linq;
using MongoDB.Bson;

namespace Sojourner.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class OrderController : ControllerBase
    {
        private OrderService _orderService;
        private UserService _userService;
        public OrderController(OrderService orderService, UserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpGet("{id:regex([[0-9a-fA-F]]{{24}})}")]
        public async Task<Order> getOrderById(string id)
        {
            var res = await _orderService.getOrderById(id);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }


        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpGet("me")]
        public async Task<List<Order>> getOrderMe()
        {
            var userId = User.Claims.Where(claim => claim.Type == "sub").FirstOrDefault().Value;
            var user = await _userService.getUserId(userId);

            var res = await _orderService.findUserActiveOrder(user.username);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }


        // GET api/v1/order/for_user?user=some@blahblah.com
        [Authorize("adminApi")]
        [HttpGet("for_user")]
        public async Task<List<Order>> getOrderByEmail(string user)
        {
            var res = await _orderService.findUserActiveOrder(user);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }


        [HttpGet("for_house")]
        public async Task<List<Order>> findHouseOrder(string oid)

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
            if (!res)
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
        [HttpDelete("{id:regex([[0-9a-fA-F]]{{24}})}")]
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

        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpGet("{oid:regex([[0-9a-fA-F]]{{24}})}/setFinished")]
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

        // [Authorize("adminApi")]
        [HttpGet("orderView")]
        public async Task<IActionResult> adminOrderView()
        {
            return Ok(await _orderService.getAdminOrderPage());
        }
    }
}

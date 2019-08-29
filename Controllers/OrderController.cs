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
        public Order findOrder(string id)
        {
            var res = _orderService.getOrderById(id);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }

        // GET api/v1/order/for_user?uid=12345
        [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
        [HttpGet("for_user")]
        public List<Order> findUserOrder(string uid = "123451234512345123451234")
        {
            var res = _orderService.findUserOrder(uid);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }

        [HttpGet("for_house")]
        public List<Order> findHouseOrder(string oid)
        {
            var res = _orderService.findHouseOrder(oid);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }

        [HttpGet("insert")]
        public IActionResult insertOrder(Order order)
        {
            order.id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            var res = _orderService.insertOrder(order);
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
        [HttpGet("delete")]
        public IActionResult deleteOrder(string oid)
        {
            var res = _orderService.getOrderById(oid);
            if (res == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { success = false, error = "order not exist" });
            }
            else
            {
                var tem = _orderService.deleteOrder(res);
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

        public IActionResult isFinishedChange(Order order)
        {
            var res = _orderService.isFinishedChange(order.id);
            if(res == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,new { success = false,error = "update isFinished failed"});
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK);
            }
        }
    }
}
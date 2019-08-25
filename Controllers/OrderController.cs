using System;
using Microsoft.AspNetCore.Mvc;
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
        public List<Order> findUserOrder(string uid)
        {
            var res = _orderService.findUserOrder(uid);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }
        public List<Order> findHouseOrder(string hid)
        {
            var res = _orderService.findHouseOrder(hid);
            if (res == null)
            {
                NotFound();
            }
            return res;
        }


        [HttpPost("/insert")]
        public String insertOrder(Order order)
        {
            var tem = _orderService.findOrder(order.id);
            if (tem != null)
            {
                return "order already exist";
            }
            else
            {
                var res = _orderService.insertOrder(order);
                if (res != null)
                {
                    return "insert order error";
                }
                else
                {
                    return "insert successful";
                }

            }

        }

    }
}
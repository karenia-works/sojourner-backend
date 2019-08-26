using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sojourner.Services;
using Sojourner.Models;
using Microsoft.AspNetCore.Authorization;
namespace Sojourner.Controllers
{
    [Authorize(IdentityServer4.IdentityServerConstants.LocalApi.PolicyName)]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private OrderService _orderService;
        public ValuesController(OrderService orderService)
        {
            _orderService = orderService;
        }
        // GET api/values

        [HttpGet]
        public ActionResult<IEnumerable<string>> test()
        {

            return new string[] { "value1", "value2", };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

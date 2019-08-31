using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sojourner.Services;
using Sojourner.Models;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
namespace Sojourner.Controllers
{
    //[Authorize(IdentityServer4.IdentityServerConstants.LocalApi.PolicyName)]
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
        [Authorize("adminApi")]
        [HttpGet("admin")]
        public ActionResult<IEnumerable<string>> test()
        {

            var client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Host = "smtp.163.com";
            client.Credentials = new System.Net.NetworkCredential("liupangx", "lzx1573");
            var Mail = new MailMessage("liupangx@163.com", "xinghuijun54@126.com");
            Mail.Subject = "House warn";
            Mail.Body = @"Hey man,
        Your house's rent time less than 7 days, please came to our web and extend your order.";
            Mail.BodyEncoding = System.Text.Encoding.UTF8;
            Mail.IsBodyHtml = true;
            client.Send(Mail);
            return new string[] { "value1", "value2", };
        }
        [Authorize(IdentityServer4.IdentityServerConstants.LocalApi.PolicyName)]
        [HttpGet("nomal")]
        public ActionResult<IEnumerable<string>> nomaltest()
        {

            return new string[] { "value1", };
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

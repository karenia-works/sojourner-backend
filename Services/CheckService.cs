using System.Threading.Tasks;
using System;
using Sojourner.Models;
using System.Net.Mail;
namespace Sojourner.Services
{
    public class CheckService
    {
        private OrderService _orderService { get; set; }
        private UserService _userService { get; set; }
        private HousesService _houseService { get; set; }
        private SmtpClient client { get; set; }
        public CheckService(OrderService orderService, UserService userService, HousesService housesService)
        {
            _orderService = orderService;
            _userService = userService;
            _houseService = housesService;
            client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Host = "smtp.163.com";
            client.Credentials = new System.Net.NetworkCredential("liupangx", "lzx1573");
            orderCheck();
        }
        async void orderCheck()
        {
            while (true)
            {
                var now = DateTime.Today;
                var outDateOrders = await _orderService.checkOrderDate(now);
                foreach (var outDateOrder in outDateOrders)
                {
                    await _orderService.isFinishedChange(outDateOrder.id);
                }
                now.AddDays(-7);
                var warnOrders = await _orderService.checkOrderDate(now);
                foreach (var warnOrder in warnOrders)
                {
                    if (warnOrder.isLongRent == true)
                    {
                        await emailGo(warnOrder);
                    }
                }
                await Task.Delay(86400 * 1000);
            }
        }
        public async Task<bool> emailGo(Order ttt)
        {
            var user = ttt.userEmail;
            var house = (await _houseService.getHouseById(ttt.houseId)).name;
            var Mail = new MailMessage("liupangx@163.com", user);
            Mail.Subject = "House warn";
            Mail.Body = @"Hey man,
        Your house's rent time less than 7 days, please came to our web and extend your order.";
            Mail.BodyEncoding = System.Text.Encoding.UTF8;
            Mail.IsBodyHtml = true;

            try { client.Send(Mail); return true; }
            catch { return false; }
        }
    }
}

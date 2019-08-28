using System.Threading.Tasks;
using System;
namespace Sojourner.Services
{
    public class CheckService
    {
        private OrderService _orderService { get; set; }
        public CheckService(OrderService orderService)
        {
            _orderService = orderService;
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
                await Task.Delay(86400 * 1000);
            }
        }
    }
}

using DACN_DVTRUCTUYEN.Models;
namespace DACN_DVTRUCTUYEN.Areas.User.Services
{
    public class OrderCancellationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        public OrderCancellationBackgroundService(IServiceProvider services)
        {
            _services = services;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000, stoppingToken); // cập nhật mỗi 10s
                using (var dataContext = _services.CreateScope().ServiceProvider.GetRequiredService<DataContext>())
                {
                    // > 15p tức quá hạn
                    var orderlist = dataContext.Orders.Where(m => m.Time.AddMinutes(15) < DateTime.Now && m.PayStatus == 0).ToList();
                    //var orderlist2 = dataContext.Orders.Where(m => m.OrderID == "20240602065405_1").FirstOrDefault();
                    foreach (var order in orderlist)
                    {
                        //trả lại số lượng hàng hóa
                        var product = dataContext.OrderDetails.Where(m => m.OrderID == order.OrderID).ToList();
                        foreach (var item in product)
                        {
                            var productoption = dataContext.ProductOptions.FirstOrDefault(m => m.ProductID == item.ProductID && m.OptionValue == item.ProductOptionValue);
                            if (productoption != null)
                            {
                                productoption.Quantity += item.Quantity;
                                dataContext.ProductOptions.Update(productoption);
                            }
                        }
                        if (order.PayStatus == 0)
                            order.PayStatus = -1;
                        dataContext.Update(order);
                        dataContext.SaveChanges();
                        Areas.TelegramBot.TelegramBotStatic.SendStaticMess(dataContext.Users.Where(m => m.UserId == order.UserID).FirstOrDefault().TelegramChatID,
                                $"Bạn có một đơn hàng quá hạn thanh toán:" +
                                $"\n\tID đơn hàng: {order.OrderID} " +
                                $"\n\tThời điểm phát sinh: {order.Time.ToString("dd/MM/yyyy HH:mm:ss")}" +
                                $"\n\tTổng số tiền thanh toán: {order.TotalPay}vnđ");
                    }
                }
            }
        }
    }
}
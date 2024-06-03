
using DACN_DVTRUCTUYEN.Areas.User.Models;
using DACN_DVTRUCTUYEN.Models;

namespace DACN_DVTRUCTUYEN.Areas.User.Services
{
    public class OrderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        public OrderBackgroundService(IServiceProvider services)
        {
            _services = services;
        }
        private static Queue<(OrderDetailView, User.Models.User)> _requestQueue = new();
        public static void EnqueueRequest((OrderDetailView, Models.User) request) => _requestQueue.Enqueue(request);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string messenger = "";
                if (_requestQueue.TryDequeue(out var request))
                {
                    using (var dataContext = _services.CreateScope().ServiceProvider.GetRequiredService<DataContext>())
                    {
                        messenger = $"DB SHOP gửi {request.Item2.Name}, đây là {request.Item1.Quantity} khóa kích hoạt/tài khoản của sản phẩm \"{request.Item1.ProductName}\" - \"{request.Item1.OptionName}\" thuộc đơn hàng \"{request.Item1.OrderID}\":";
                        var key = dataContext.Product_Keys.Where(m => m.OrderID == "0").Take(request.Item1.Quantity).ToList();
                        foreach (var item2 in key)
                        {
                            item2.OrderID = request.Item1.OrderID;
                            messenger += $"\n\"{item2.Key1}\" - \"{item2.Key2}\"";
                            dataContext.Product_Keys.Update(item2);
                        }
                        dataContext.OrderDetails.Update(new OrderDetail()
                        {
                            Amount = request.Item1.Amount,
                            ProductID = request.Item1.ProductID,
                            ProductOptionValue = request.Item1.ProductOptionValue,
                            OrderID = request.Item1.OrderID,
                            OrderStatusID = 3,
                            Quantity = request.Item1.Quantity,
                        });
                        dataContext.SaveChanges();
                        TelegramBot.TelegramBotStatic.SendStaticMess(request.Item2.TelegramChatID, messenger);
                    }
                }
                else
                {
                    await Task.Delay(10000, stoppingToken); // nếu ko có phần từ nào trong hàng đợi thì đợi 10s
                }
            }
        }
    }
}
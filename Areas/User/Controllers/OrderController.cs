using DACN_DVTRUCTUYEN.Models;
using DACN_DVTRUCTUYEN.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Collections.Concurrent;

namespace DACN_DVTRUCTUYEN.Areas.User.Controllers
{
    [Area("User")]
    public class OrderBackgroundService : BackgroundService
    {
        private readonly ConcurrentQueue<(string,string)> _requestQueue = new();

        public void EnqueueRequest((string,string) request) => _requestQueue.Enqueue(request);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_requestQueue.TryDequeue(out var request))
                {
                    // Xử lý yêu cầu 'request' ở đây
                }
                else
                {
                    await Task.Delay(100, stoppingToken); // Tránh vòng lặp bận
                }
            }
        }
    }

    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("/user/Orders/OK/{orderid}&{vnp_transid}&{orderInfor}")]
        [HttpGet]
        public async Task<IActionResult> Pay_return_OK(string orderid, string vnp_transid, string orderInfor)
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return BadRequest();
            }
            //update orders
            var value = _dataContext.Orders.Where(m => m.OrderID == orderid && m.UserID == userid).FirstOrDefault();
            if (value == null)
                return BadRequest();
            if (!long.TryParse(vnp_transid, out long trans_no)) 
                return BadRequest();
            value.PayStatus = 1;
            value.TransactionNo = trans_no;
            _dataContext.Update(value);
            //del user cart
            var listcart = _dataContext.CartViews.Where(m => m.UserID == userid && m.ProductOptionQuantity > 0).ToList();
            foreach (var item in listcart)
            {
                _dataContext.Carts.Where(m => m.UserID == value.UserID && m.ProductID == item.ProductID && m.ProductOptionValue == item.OptionValue).ExecuteDelete();
            }
            //change user totalpaid
            var user_data = _dataContext.Users.Where(m => m.UserId == value.UserID).FirstOrDefault();
            if (user_data == null)
                return BadRequest();
            user_data.TotalPaid += value.TotalPay;
            _dataContext.Update(user_data);
            //save
            _dataContext.SaveChanges();
            return Redirect("/user/ordershistory");
        }
        [Route("/user/Orders/Error/{orderid}&{vnp_transid}&{orderInfor}")]
        [HttpGet]
        public async Task<IActionResult> Pay_return_Error(string orderid, string vnp_transid, string orderInfor)
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return BadRequest();
            }
            var value = _dataContext.Orders.Where(m => m.OrderID == orderid).FirstOrDefault();
            if (value == null)
                return BadRequest();
            if (!long.TryParse(vnp_transid, out long trans_no))
                return BadRequest();
            value.PayStatus = -1;
            value.TransactionNo = trans_no;
            TelegramBot.TelegramBotStatic.SendStaticMess(_dataContext.Users.FirstOrDefault(m=> m.UserId == value.UserID).TelegramChatID,$"Thanh toán thất bại tại đơn hàng {value.OrderID}");
            _dataContext.Update(value);
            _dataContext.SaveChanges();
            return Redirect("/user/ordershistory");
        }

        //[Route("/user/orders/cancel/{orderid}")]
        //MOVE TO CartController
    }
}

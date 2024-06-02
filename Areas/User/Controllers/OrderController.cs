using DACN_DVTRUCTUYEN.Areas.User.Models;
using DACN_DVTRUCTUYEN.Models;
using DACN_DVTRUCTUYEN.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Collections.Concurrent;
using Telegram.Bot.Types;

namespace DACN_DVTRUCTUYEN.Areas.User.Controllers
{
    [Area("User")]
    public class OrderBackgroundService : BackgroundService
    {
        private readonly ConcurrentQueue<(string, string)> _requestQueue = new();

        public void EnqueueRequest((string, string) request) => _requestQueue.Enqueue(request);

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

    [Area("User")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("/User/orders")]
        public async Task<IActionResult> Index()
        {
            return Redirect("/user/");
        }
        [Route("/user/Orders/OK/{orderid}&{vnp_transid}&{orderInfor}")]
        [HttpGet]
        public async Task<IActionResult> Pay_return_OK(string orderid, string vnp_transid, string orderInfor)
        {
            var value = _dataContext.Orders.Where(m => m.OrderID == orderid && m.PayStatus == 0).FirstOrDefault();
            if (value == null)
                return Redirect("/user/");
            var user_data = _dataContext.Users.Where(m => m.UserId == value.UserID).FirstOrDefault();
            if (user_data == null)
                return Redirect("/user/");
            //send mess
            TelegramBot.TelegramBotStatic.SendStaticMess(user_data.TelegramChatID, $"Chào {user_data.Name}\n\tBạn vừa thanh toán cho đơn hàng {orderid}");
            //update orders
            if (!long.TryParse(vnp_transid, out long trans_no))
                return Redirect("/user/");
            value.PayStatus = 1;
            value.TransactionNo = trans_no;
            _dataContext.Update(value);
            //lấy key cho người dùng
            var product = _dataContext.OrderDetailViews.Where(m => m.OrderID == orderid).ToList();
            foreach (var item in product)
            {
                if (item.Type == 1)
                {
                    var key = _dataContext.Product_Keys.Where(m => m.OrderID == "0").Take(item.Quantity).ToList();
                    string messenger = $"DB SHOP gửi {user_data.Name}, đây là {item.Quantity} khóa kích hoạt/tài khoản của sản phẩm \"{item.ProductName}\" - \"{item.OptionName}\":";
                    foreach (var item2 in key)
                    {
                        item2.OrderID = orderid;
                        messenger += $"\n{item2.Key1} - {item2.Key2}";
                        _dataContext.Product_Keys.Update(item2);
                    }
                    _dataContext.OrderDetails.Update(new OrderDetail()
                    {
                        Amount = item.Amount,
                        ProductID = item.ProductID,
                        ProductOptionValue = item.ProductOptionValue,
                        OrderID = item.OrderID,
                        OrderStatusID = 3,
                        Quantity = item.Quantity,
                    });
                    TelegramBot.TelegramBotStatic.SendStaticMess(user_data.TelegramChatID, messenger);
                }
                else
                {
                    _dataContext.OrderDetails.Update(new OrderDetail()
                    {
                        Amount = item.Amount,
                        ProductID = item.ProductID,
                        ProductOptionValue = item.ProductOptionValue,
                        OrderID = item.OrderID,
                        OrderStatusID = 1,
                        Quantity = item.Quantity,
                    });
                }
            }
            //del user cart
            var listcart = _dataContext.CartViews.Where(m => m.UserID == value.UserID && m.ProductOptionQuantity > 0).ToList();
            foreach (var item in listcart)
            {
                _dataContext.Carts.Where(m => m.UserID == value.UserID && m.ProductID == item.ProductID && m.ProductOptionValue == item.OptionValue).ExecuteDelete();
            }
            //change user totalpaid
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
            var value = _dataContext.Orders.Where(m => m.OrderID == orderid).FirstOrDefault();
            if (value == null)
                return Redirect("/user/");
            if (!long.TryParse(vnp_transid, out long trans_no))
                return Redirect("/user/");
            value.PayStatus = -1;
            value.TransactionNo = trans_no;
            var product = _dataContext.OrderDetails.Where(m => m.OrderID == orderid).ToList();
            foreach (var item in product)
            {
                var productoption = _dataContext.ProductOptions.FirstOrDefault(m => m.ProductID == item.ProductID && m.OptionValue == item.ProductOptionValue);
                if (productoption != null)
                {
                    productoption.Quantity += item.Quantity;
                    _dataContext.ProductOptions.Update(productoption);
                }
            }
            TelegramBot.TelegramBotStatic.SendStaticMess(_dataContext.Users.FirstOrDefault(m => m.UserId == value.UserID).TelegramChatID, $"Thanh toán thất bại tại đơn hàng {value.OrderID}");
            _dataContext.Update(value);
            _dataContext.SaveChanges();
            return Redirect("/user/ordershistory");
        }
        [Route("/user/Orders/Detail/{orderid}")]
        [HttpGet]
        public async Task<IActionResult> Detail(string orderid)
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return Redirect("/user/");
            }
            var value = _dataContext.Orders.Where(m => m.OrderID == orderid && m.UserID == userid).FirstOrDefault();
            if (value == null)
                return Redirect("/user/");
            return View((value, _dataContext.OrderDetailViews.Where(m => m.OrderID == value.OrderID).ToList()));
        }
        [Route("/user/orders/detail/api/{orderid}/{productid}/{optionvalue}")]
        [HttpGet]
        public async Task<IActionResult> GETDETAIL(string orderid,string productid,string optionvalue)
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return BadRequest();
            }
            var value = _dataContext.Orders.FirstOrDefault(m => m.UserID == userid && m.OrderID == orderid);
            if (value == null)
                return BadRequest();
            return Ok(_dataContext.Product_Keys.Where(m => m.OrderID == orderid && m.ProductID == productid && m.OptionValue == optionvalue).ToList());
        }
        [Route("/user/orders/error-rp/{orderid}&{productid}&{optionvalue}&{key1}")]
        [HttpGet]
        public async Task<IActionResult> ERROR_REPORT(string orderid, string productid, string optionvalue)
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return Redirect("/user/");
            }
            var value = _dataContext.Orders.FirstOrDefault(m => m.UserID == userid && m.OrderID == orderid);
            if (value == null)
                return Redirect("/user/");
            return View((_dataContext.Product_Keys.FirstOrDefault(m => m.OrderID == orderid && m.ProductID == productid && m.OptionValue == optionvalue), _dataContext.OrderDetailViews.FirstOrDefault(m => m.ProductID == productid && m.ProductOptionValue == optionvalue)));
        }
        [Route("/user/orders/error-rp/{orderid}&{productid}&{optionvalue}&{key1}")]
        [HttpPost]
        public async Task<IActionResult> ERROR_REPORT(string orderid, string productid, string optionvalue,string userreport)
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return Redirect("/user/");
            }
            var value = _dataContext.Orders.FirstOrDefault(m => m.UserID == userid && m.OrderID == orderid);
            if (value == null)
                return Redirect("/user/");
            var thisorderdetail = _dataContext.OrderDetailViews.FirstOrDefault(m => m.OrderID == orderid && m.ProductID == productid && m.ProductOptionValue == optionvalue);
            if (thisorderdetail == null)
                return Redirect("/user/");
            var thiskey = _dataContext.Product_Keys.FirstOrDefault(m => m.OrderID == orderid && m.ProductID == productid && m.OptionValue == optionvalue);
            if (thiskey == null)
                return Redirect("/user/");
            thiskey.Rp_FromUser = userreport;
            thiskey.Rp_FromUser_Time = DateTime.Now;
            _dataContext.Update(thiskey);
            _dataContext.SaveChanges();
            return View((thiskey,thisorderdetail));
        }
        //[Route("/user/orders/cancel/{orderid}")]
        //MOVE TO CartController
    }
}

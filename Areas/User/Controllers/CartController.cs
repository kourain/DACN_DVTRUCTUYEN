using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using DACN_DVTRUCTUYEN.Utilities;
using DACN_DVTRUCTUYEN.Areas.User.Models;
using Hangfire;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
namespace DACN_DVTRUCTUYEN.Areas.User.Controllers
{
    [Area("User")]
    public class CartController : Controller
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        private readonly DataContext _dataContext;
        public CartController(DataContext dataContext, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
            _dataContext = dataContext;
        }
        [Route("/User/cart")]
        public async Task<IActionResult> Index()
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                Redirect("/user/");
            }
            return View(_dataContext.CartViews.Where(m => m.UserID == userid).ToList());
        }
        [Route("/User/cart/add&{productid}&{productoptionvalue}")]
        public IActionResult AddToCart(string productid, string productoptionvalue)
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                Redirect("/user/");
            }
            if (_dataContext.Carts.Where(m => m.UserID == userid && m.ProductID == productid && m.ProductOptionValue == productoptionvalue).FirstOrDefault() != null)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = "Sản phẩm đã tồn tại trong giỏ hàng, vui lòng xác nhận"
                });
            }
            var item = new DACN_DVTRUCTUYEN.Areas.User.Models.Cart()
            {
                UserID = userid,
                ProductID = productid,
                ProductOptionValue = productoptionvalue,
                Quantity = 1
            };
            _dataContext.Add(item);
            _dataContext.SaveChanges();
            return Ok(new
            {
                code = 1,
                messenger = "Đã thêm vào giỏ hàng"
            });
        }
        [Route("/user/cart/{ProductID}")]
        public async Task<IActionResult> Detail(string ProductID)
        {
            var cart = _dataContext.Products.Where(m => m.ProductID == ProductID).FirstOrDefault();
            if (cart == null)
            {
                return Redirect("/User");
            }
            return View(cart);
        }
        [Route("/user/cart/pay")]
        public IActionResult Pay()
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return BadRequest();
            }
            var userinf = _dataContext.Users.FirstOrDefault(m => m.UserId == userid);
            if (userinf == null)
            {
                return BadRequest();
            }
            if (userinf.Ban == true)
            {
                return BadRequest();
            }
            if (userinf.TelegramChatID == null || string.IsNullOrEmpty(userinf.TelegramName))
            {
                return Ok(new
                {
                    code = 0,
                    messenger = $"Vui lòng liên kết với tài khoản Telegram để nhận thông báo!!!"
                });
            }
            //còn đơn chưa thanh toán
            var orderold = _dataContext.Orders.Where(m => m.UserID == userid && m.PayStatus == 0).FirstOrDefault();
            if (orderold != null)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = $"Bạn có đơn hàng chưa hoàn thành thanh toán tại VNPay({orderold.Time.ToString("hh:mm:ss dd/MM/yyyy")}), vui lòng kiểm tra tại lịch sử mua hàng!"
                });
            }
            //make value
            var now = DateTime.Now;
            string orderid = now.ToString("yyyyMMddhhmmss") + "_" + userid.ToString();
            var listcart = _dataContext.CartViews.Where(m => m.UserID == userid && m.ProductOptionQuantity > 0).ToList();
            if (listcart.Count == 0)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = $"Không có sản phẩm nào hợp lệ trong giỏ hàng của bạn!"
                });
            }
            int totalpay = 0;
            var infor = "";
            foreach (var item in listcart)
            {
                totalpay += item.PriceNow;
                infor += item.ProductID + "_" + item.OptionValue + ",";
            }
            infor = infor.Substring(0, infor.Length - 1);
            // add new order 
            Order neworder = new Order()
            {
                OrderID = orderid,
                UserID = userid,
                TotalPay = totalpay,
                Time = now,
                PayStatus = 0,
            };
            _dataContext.Add(neworder);
            // add order infor
            foreach (var item in listcart)
            {
                _dataContext.OrderDetails.Add(new OrderDetail()
                {
                    OrderID = orderid,
                    ProductID = item.ProductID,
                    ProductOptionValue = item.OptionValue,
                    Amount = item.PriceNow,
                    OrderStatusID = 1,
                });
            }
            //lên lịch 15phút kể từ lúc tạo thanh toán đến lúc hết thời hạn thanh toán(15phút theo VNPay)
            Areas.TelegramBot.TelegramBotStatic.SendStaticMess(_dataContext.Users.Where(m => m.UserId == neworder.UserID).FirstOrDefault().TelegramChatID,
                $"Bạn có một đơn hàng cần thanh toán mới: " +
                $"\n\tID đơn hàng: {neworder.OrderID} " +
                $"\n\tThời điểm phát sinh: {neworder.Time.ToString("dd/MM/yyyy hh:mm:ss")}" +
                $"\n\tTổng số tiền cần thanh toán: {neworder.TotalPay}vnđ");
            _backgroundJobClient.Schedule(neworder.OrderID, () => Pay_Cancel(orderid), TimeSpan.FromHours(7.25));
            _dataContext.SaveChanges();
            return Ok(new
            {
                code = 1,
                messenger = $"/vnpayapi/{totalpay}00&{infor}&{orderid}"
            });
        }
        [Route("/user/orders/cancel/{orderid}")]
        public async Task<IActionResult> order_cancel(string orderid)
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return NotFound();
            }
            var value = _dataContext.Orders.Where(m => m.OrderID == orderid && m.UserID == userid).FirstOrDefault();
            if (value != null)
            {
                if (value.PayStatus == 0)
                    value.PayStatus = -2;
                _dataContext.Update(value);
                _dataContext.SaveChanges();
                TelegramBot.TelegramBotStatic.SendStaticMess(_dataContext.Users.FirstOrDefault(m => m.UserId == value.UserID).TelegramChatID, $"Bạn đã hủy thanh toán đối với đơn hàng đơn hàng {value.OrderID}");
            }
            return Redirect("/user/OrdersHistory");
        }
        //xử lý việc người dùng thoát trang thanh toán
        public void Pay_Cancel(string orderid)
        {
            // Thực hiện logic hủy đơn hàng ở đây
            var value = _dataContext.Orders.Where(m => m.OrderID == orderid).FirstOrDefault();
            if (value != null)
            {
                if (value.PayStatus == 0)
                    value.PayStatus = -1;
                _dataContext.Update(value);
                _dataContext.SaveChanges();
                Areas.TelegramBot.TelegramBotStatic.SendStaticMess(_dataContext.Users.Where(m => m.UserId == value.UserID).FirstOrDefault().TelegramChatID,
                    $"Bạn có một đơn hàng quá hạn thanh toán:" +
                    $"\n\tID đơn hàng: {value.OrderID} " +
                    $"\n\tThời điểm phát sinh: {value.Time.ToString("dd/MM/yyyy hh:mm:ss")}" +
                    $"\n\tTổng số tiền thanh toán: {value.TotalPay}vnđ");
            }
        }

        [Route("/user/cart/getProductOption/{ProductID}")]
        public IActionResult getcartOption(string ProductID)
        {
            var query = (from m in _dataContext.ProductOptions.Where(m => m.ProductID == ProductID)
                         select new ProductOption()
                         {
                             OptionName = m.OptionName,
                             OptionValue = m.OptionValue,
                             ProductID = m.ProductID,
                             PriceNow = m.PriceNow,
                             PriceOld = m.PriceOld,
                             Quantity = m.Quantity,
                             SoldCount = m.SoldCount,
                             UseUserAccount = m.UseUserAccount,
                         });
            return Ok(query);
        }

        [Route("/user/cart/{ProductID}/{ProductOptionValue}")]
        public async Task<IActionResult> Detail(string ProductID, string ProductOptionValue)
        {
            var cart = _dataContext.Products.Where(m => m.ProductID == ProductID).FirstOrDefault();
            if (cart == null)
            {
                return Redirect("/User");
            }
            ViewBag.ProductOption0 = ProductOptionValue;
            return View(cart);
        }
        [Route("/user/cart/get_cart_count")]
        public IActionResult getcartCount()
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return BadRequest();
            }
            return Ok(new
            {
                code = 1,
                messenger = _dataContext.CartViews.Where(m => m.UserID == userid && m.ProductOptionQuantity > 0).Count(),
            });
        }
        [Route("/user/cart/delproduct/{productid}")]
        public async Task<IActionResult> delproduct(string productid)
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(productid)) { return BadRequest(); }
            _dataContext.Carts.Where(m => m.UserID == userid && m.ProductID == productid).ExecuteDelete();
            _dataContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}

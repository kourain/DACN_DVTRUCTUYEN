using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using DACN_DVTRUCTUYEN.Utilities;
using DACN_DVTRUCTUYEN.Areas.User.Models;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.Payments;
using System.Threading;
namespace DACN_DVTRUCTUYEN.Areas.User.Controllers
{
    [Area("User")]
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext dataContext)
        {
            //, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager
            //    _backgroundJobClient = backgroundJobClient;
            //    _recurringJobManager = recurringJobManager;
            _dataContext = dataContext;
        }
        [Route("/User/cart")]
        public async Task<IActionResult> Index()
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return Redirect("/user/");
            }
            var cartitem = _dataContext.CartViews.Where(m => m.UserID == userid).ToList();
            foreach (var item in cartitem)
            {
                if (item.Quantity > item.ProductOptionQuantity)
                {
                    item.Quantity = item.ProductOptionQuantity;
                    var item2 = new Cart()
                    {
                        UserID = userid,
                        Quantity = item.ProductOptionQuantity,
                        CreateDate = item.CreateDate,
                        ProductID = item.ProductID,
                        ProductOptionValue = item.OptionValue,
                    };
                    _dataContext.Update(item2);
                }

            }
            _dataContext.SaveChanges();
            return View(cartitem);
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
            var item = new Cart()
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
        [Route("/user/cart/pay")]
        [HttpPost]
        public async Task<IActionResult> Pay(string note)
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
            if (!((userinf.TelegramChatID == null && string.IsNullOrEmpty(userinf.TelegramName)) || string.IsNullOrEmpty(userinf.Phone)))
            {
                return Ok(new
                {
                    code = 0,
                    messenger = $"Vui lòng liên kết với tài khoản Telegram hoặc thêm số điện thoại !!!"
                });
            }
            //còn đơn chưa thanh toán
            var orderold = _dataContext.Orders.Where(m => m.UserID == userid && m.PayStatus == 0).FirstOrDefault();
            if (orderold != null)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = $"Bạn có đơn hàng chưa hoàn thành thanh toán tại VNPay(lúc {orderold.Time.ToString("HH:mm:ss dd/MM/yyyy")}), vui lòng kiểm tra tại lịch sử mua hàng!"
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
                totalpay += item.PriceNow * item.Quantity;
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
                Note = note
            };
            _dataContext.Add(neworder);
            // add order infor
            foreach (var item in listcart)
            {
                //cập nhật số lượng của sản phẩm trước để giữ chỗ cho lượt mua này
                var productoption = _dataContext.ProductOptions.FirstOrDefault(m => m.ProductID == item.ProductID && m.OptionValue == item.OptionValue);
                if (productoption == null) continue;
                if (productoption.Quantity < item.Quantity) item.Quantity = productoption.Quantity;
                productoption.Quantity -= item.Quantity;
                _dataContext.ProductOptions.Update(productoption);
                _dataContext.OrderDetails.Add(new OrderDetail()
                {
                    OrderID = orderid,
                    ProductID = item.ProductID,
                    ProductOptionValue = item.OptionValue,
                    Amount = item.PriceNow,
                    OrderStatusID = 0,
                    Quantity = item.Quantity,
                });
            }
            //lên lịch 15phút kể từ lúc tạo thanh toán đến lúc hết thời hạn thanh toán(15phút theo VNPay)
            //OrderCancellationBackgroundService.addQueue(orderid);
            //chuyển sang background service
            _dataContext.SaveChanges();
            Areas.TelegramBot.Controllers.HomeController.SendMess(_dataContext.Users.Where(m => m.UserId == neworder.UserID).FirstOrDefault().TelegramChatID,
                $"Bạn có một đơn hàng cần thanh toán mới: " +
                $"\n\tID đơn hàng: {neworder.OrderID} " +
                $"\n\tThời điểm phát sinh: {neworder.Time.ToString("dd/MM/yyyy HH:mm:ss")}" +
                $"\n\tTổng số tiền cần thanh toán: {neworder.TotalPay}vnđ");
            return Ok(new
            {
                code = 1,
                messenger = VNPayAPI.Controllers.HomeController.Payment($"{totalpay}00", infor, orderid)
            });
        }


        [Route("/user/orders/cancel/{orderid}")]
        public async Task<IActionResult> order_cancel(string orderid)
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return Redirect("/user/");
            }
            var value = _dataContext.Orders.Where(m => m.OrderID == orderid && m.UserID == userid).FirstOrDefault();
            if (value != null)
            {
                //trả lại số lượng hàng hóa
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
                if (value.PayStatus == 0)
                    value.PayStatus = -2;
                _dataContext.Update(value);
                _dataContext.SaveChanges();
                TelegramBot.Controllers.HomeController.SendMess(_dataContext.Users.FirstOrDefault(m => m.UserId == value.UserID).TelegramChatID, $"Bạn đã hủy thanh toán đối với đơn hàng đơn hàng {value.OrderID}");
            }
            return Redirect("/user/OrdersHistory");
        }

        [Route("/user/cart/getProductOption/{ProductID}")]
        public async Task<IActionResult> getcartOption(string ProductID)
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
                             Type = m.Type,
                         });
            return Ok(query);
        }

        [Route("/user/cart/get_cart_count")]
        public async Task<IActionResult> getcartCount()
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
        [Route("/user/cart/delproduct/{productid}/{optionvalue}")]
        public async Task<IActionResult> delproduct(string productid, string optionvalue)
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(productid)) { return BadRequest(); }
            _dataContext.Carts.Where(m => m.UserID == userid && m.ProductID == productid && m.ProductOptionValue == optionvalue).ExecuteDelete();
            _dataContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [Route("/User/cart/add-{productid}-{optionvalue}")]
        public async Task<IActionResult> add(string productid, string optionvalue)
        {
            productid = productid.ToLower();
            optionvalue = optionvalue.ToLower();
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return BadRequest();
            }
            var item = _dataContext.CartViews.Where(m => m.UserID == userid && m.ProductID == productid && m.OptionValue == m.OptionValue).FirstOrDefault();
            if (item == null)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = "Yêu cầu không hợp lệ, vui lòng làm mới trang giỏ hàng"
                });
            }
            //if (item.Type != 1)
            //    return Ok(new
            //    {
            //        code = 0,
            //        messenger = "Yêu cầu không hợp lệ, vui lòng làm mới trang giỏ hàng"
            //    });
            if (item.ProductOptionQuantity == item.Quantity)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = "Đạt giới hạn sản phẩm"
                });
            }
            var cart = new Cart()
            {
                UserID = item.UserID,
                ProductID = item.ProductID,
                ProductOptionValue = item.OptionValue,
                Quantity = item.Quantity + 1,
                CreateDate = item.CreateDate,
            };
            _dataContext.Update(cart);
            _dataContext.SaveChanges();
            return Ok(new
            {
                code = 1
            });
        }
        [Route("/User/cart/subtract-{productid}-{optionvalue}")]
        public async Task<IActionResult> subtract(string productid, string optionvalue)
        {
            productid = productid.ToLower();
            optionvalue = optionvalue.ToLower();
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return BadRequest();
            }
            var item = _dataContext.CartViews.Where(m => m.UserID == userid && m.ProductID == productid && m.OptionValue == m.OptionValue).FirstOrDefault();
            if (item == null)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = "Yêu cầu không hợp lệ, vui lòng làm mới trang giỏ hàng"
                });
            }
            //if (item.Type != 1)
            //    return Ok(new
            //    {
            //        code = 0,
            //        messenger = "Yêu cầu không hợp lệ, vui lòng làm mới trang giỏ hàng"
            //    });
            if (1 == item.Quantity)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = "Tối thiểu tồn tại 1 sản phẩm"
                });
            }
            var cart = new Cart()
            {
                UserID = item.UserID,
                ProductID = item.ProductID,
                ProductOptionValue = item.OptionValue,
                Quantity = item.Quantity - 1,
                CreateDate = item.CreateDate,
            };
            _dataContext.Update(cart);
            _dataContext.SaveChanges();
            return Ok(new
            {
                code = 1
            });
        }
    }
}

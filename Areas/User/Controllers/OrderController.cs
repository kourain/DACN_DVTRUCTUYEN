using DACN_DVTRUCTUYEN.Areas.User.Models;
using DACN_DVTRUCTUYEN.Areas.User.Services;
using DACN_DVTRUCTUYEN.Models;
using DACN_DVTRUCTUYEN.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Telegram.Bot.Types;

namespace DACN_DVTRUCTUYEN.Areas.User.Controllers
{
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
        [Route("/user/Orders/OK/{orderid}&{vnp_transid:long}&{orderInfor}")]
        [HttpGet]
        public async Task<IActionResult> Pay_return_OK(string orderid, long trans_no, string orderInfor)
        {
            //update orders
            var value = _dataContext.Orders.Where(m => m.OrderID == orderid && m.PayStatus == 1 && m.TransactionNo == trans_no).FirstOrDefault();
            if (value == null)
                return Redirect("/user/");
            value.PayStatus = 2;
            _dataContext.Update(value);
            _dataContext.SaveChanges();

            var user = _dataContext.Users.Where(m => m.UserId == value.UserID).FirstOrDefault();
            if (user == null)
                return Redirect("/user/");
            //lấy key cho người dùng
            var product = _dataContext.OrderDetailViews.Where(m => m.OrderID == orderid).ToList();
            foreach (var item in product)
            {
                if (item.Type == 1)
                {
                    OrderBackgroundService.EnqueueRequest((item, user));
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
                var num = _dataContext.ProductOptions.FirstOrDefault(m => m.ProductID == item.ProductID && m.OptionValue == item.ProductOptionValue);
                num.SoldCount += item.Quantity;
                _dataContext.Update(num);
            }
            //del user cart
            var listcart = _dataContext.CartViews.Where(m => m.UserID == value.UserID && m.ProductOptionQuantity > 0).ToList();
            foreach (var item in listcart)
            {
                _dataContext.Carts.Where(m => m.UserID == value.UserID && m.ProductID == item.ProductID && m.ProductOptionValue == item.OptionValue).ExecuteDelete();
            }
            //change user totalpaid
            user.TotalPaid += value.TotalPay;
            _dataContext.Update(user);
            //save
            _dataContext.SaveChanges();
            return Redirect("/user/ordershistory");
        }
        [Route("/user/Orders/Error/{orderid}&{vnp_transid}&{orderInfor}")]
        [HttpGet]
        public async Task<IActionResult> Pay_return_Error(string orderid, string vnp_transid, string orderInfor)
        {
            if (!long.TryParse(vnp_transid, out long trans_no))
                return Redirect("/user/");
            var value = _dataContext.Orders.Where(m => m.OrderID == orderid && m.TransactionNo == trans_no && m.PayStatus == -1).FirstOrDefault();
            if (value == null)
                return Redirect("/user/");
            value.PayStatus = -2;
            _dataContext.Update(value);
            _dataContext.SaveChanges();

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
        public async Task<IActionResult> GETDETAIL(string orderid, string productid, string optionvalue)
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
            var value = _dataContext.Orders.FirstOrDefault(m => m.UserID == userid && m.OrderID == orderid && m.Time.AddDays(7) > DateTime.Now);
            if (value == null)
                return Redirect("/user/");
            return View((_dataContext.Product_Keys.FirstOrDefault(m => m.OrderID == orderid && m.ProductID == productid && m.OptionValue == optionvalue), _dataContext.OrderDetailViews.FirstOrDefault(m => m.OrderID == orderid && m.ProductID == productid && m.ProductOptionValue == optionvalue)));
        }
        [Route("/user/orders/error-rp/{orderid}&{productid}&{optionvalue}&{key1}")]
        [HttpPost]
        public async Task<IActionResult> ERROR_REPORT(string orderid, string productid, string optionvalue, string userreport)
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return Redirect("/user/");
            }
            var value = _dataContext.Orders.FirstOrDefault(m => m.UserID == userid && m.OrderID == orderid && m.Time.AddDays(7) > DateTime.Now);
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
            return View((thiskey, thisorderdetail));
        }
        //[Route("/user/orders/cancel/{orderid}")]
        //MOVE TO CartController
    }
}

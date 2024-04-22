using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.Security.Cryptography;
using System;
using System.Security.AccessControl;
using System.Web;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using DACN_DVTRUCTUYEN.Utilities;
using DACN_DVTRUCTUYEN.Areas.User.Models;
namespace DACN_DVTRUCTUYEN.Areas.User.Controllers
{
    [Area("User")]
    public class Cart : Controller
    {
        private readonly DataContext _dataContext;
        public Cart(DataContext dataContext)
        {
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
            return View(_dataContext.CartViews.Where(m=>m.UserID == userid).ToList());
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
            var listcart = _dataContext.CartViews.Where(m => m.UserID == userid && m.ProductOptionQuantity >0).ToList();
            int totalpay = 0;
            var infor = "";
            foreach (var item in listcart)
            {
                totalpay += item.PriceNow;
                infor += item.ProductID+ "_" + item.OptionValue;
            }
            var now = DateTime.Now;
            Order neworder = new Order()
            {
                UserID = userid,
                TotalPay = totalpay,
                Time = now
            };
            _dataContext.Add(neworder);
            _dataContext.SaveChanges();
            var check = _dataContext.Orders.Where(m => m.UserID == userid && m.TotalPay == totalpay && m.Time == now).FirstOrDefault();
            if(check == null)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = "Vui lòng cập nhật lại giỏ hàng"
                });
            }
            var orderid = check.OrderID;
            var n = from m in listcart
                    select _dataContext.Add(new OrderDetail()
                    {
                        OrderID = orderid,
                        ProductID = m.ProductID,
                        ProductOptionValue = m.OptionValue,
                        OrderStatus = 1,
                    });
            _dataContext.SaveChanges();
            return Ok(new
            {
                code = 1,
                messenger = $"/vnpayapi/{totalpay}00&{infor}&{orderid}"
            });
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
                messenger = _dataContext.CartViews.Where(m => m.UserID == userid && m.ProductOptionQuantity>0).Count(),
            });
        }
    }
}

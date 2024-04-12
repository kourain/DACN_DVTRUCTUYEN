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

            Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]);
            var cart = _dataContext.Carts.Where(m => m.UserID == int.Parse(Request.Cookies["id"])).FirstOrDefault();
            return View(cart);
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
    }
}

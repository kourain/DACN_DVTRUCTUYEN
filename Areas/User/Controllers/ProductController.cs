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
namespace DACN_DVTRUCTUYEN.Areas.User.Controllers
{
    [Area("User")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("/User/product")]
        public async Task<IActionResult> Index()
        {
            return Redirect("/User");
        }
        [Route("/user/product/{ProductID}")]
        public async Task<IActionResult> Detail(string ProductID)
        {
            var product = _dataContext.Products.Where(m => m.ProductID == ProductID).FirstOrDefault();
            if (product == null)
            {
                return Redirect("/User");
            }
            return View(product);
        }

        [Route("/user/product/getProductOption/{ProductID}")]
        public IActionResult getproductOption(string ProductID)
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

        [Route("/user/product/{ProductID}/{ProductOptionValue}")]
        public async Task<IActionResult> Detail(string ProductID, string ProductOptionValue)
        {
            var product = _dataContext.Products.Where(m => m.ProductID == ProductID).FirstOrDefault();
            if (product == null)
            {
                return Redirect("/User");
            }
            ViewBag.ProductOption0 = ProductOptionValue;
            return View(product);
        }
    }
}

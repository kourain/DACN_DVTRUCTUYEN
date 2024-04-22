using DACN_DVTRUCTUYEN.Areas.User.Models;
using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;
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
            var product = _dataContext.ProductViews.Where(m => m.ProductID == ProductID).ToList(); 
            if (product == null || product.Count ==0)
            {
                return Redirect("/User");
            }
            ViewBag.ProductOption0 = product[0].OptionValue;
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
                             CreateDate = m.CreateDate
                         });
            return Ok(query);
        }

        [Route("/user/product/getProduct/top/{skip:int}")]
        public IActionResult getproductTop(int skip)
        {
            if (skip == null) { skip = 0; }
            //iqueriable join
            var list = _dataContext.ProductOptions.OrderByDescending(m => m.SoldCount).Skip(skip * 8).Take(8).Join(_dataContext.Products, m => m.ProductID, n => n.ProductID, (m, n) => new ProductView()
            {
                ProductID = m.ProductID,
                ProductName = n.ProductName,
                ProductImg = n.ProductImg,
                ProductDescription = n.ProductDescription,
                OptionName = m.OptionName,
                OptionValue = m.OptionValue,
                PriceOld = m.PriceOld,
                PriceNow = m.PriceNow,
            }).GroupBy(p => p.ProductID).Select(g => g.First()).ToList(); // loại bỏ các product trùng nhau

            return Ok(list);
        }

        [Route("/user/product/{ProductID}/{ProductOptionValue}")]
        public async Task<IActionResult> Detail(string ProductID, string ProductOptionValue)
        {
            var product = _dataContext.ProductViews.Where(m => m.ProductID == ProductID).ToList();
            if (product == null || product.Count == 0)
            {
                return Redirect("/User");
            }
            ViewBag.ProductOption0 = ProductOptionValue;
            return View(product);
        }
        [Route("/user/product/inc/{ProductID}")]
        public IActionResult INC(string ProductID)
        {
            var product = _dataContext.ProductViews.Where(m => m.ProductID == ProductID).ToList();
            if (product == null || product.Count == 0)
            {
                return BadRequest();
            }
            var num = _dataContext.Database.ExecuteSql(FormattableStringFactory.Create($"EXEC [dbo].[INC_PRODUCT_VIEWCOUNT] '{ProductID}'"));
            return Ok(num);
        }
    }
}

using DACN_DVTRUCTUYEN.Areas.User.Models;
using DACN_DVTRUCTUYEN.Models;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Mvc;

namespace DACN_DVTRUCTUYEN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("/admin/order")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Redirect("/admin/order/Come");
        }
        [Route("/admin/Order/Come")]
        [HttpGet]
        public async Task<IActionResult> Come()
        {
            var list_order = _dataContext.OrderReportViews.Where(m => m.PayStatus == 2 && m.Type != 1 && m.OrderStatusID == 1).ToList();

            return View(list_order);
        }
        [Route("/admin/Order/processing")]
        [HttpGet]
        public async Task<IActionResult> Processing()
        {
            var list_order = _dataContext.OrderReportViews.Where(m => m.PayStatus == 2 && m.Type != 1 && m.OrderStatusID == 2).ToList();

            return View(list_order);
        }
        [Route("/admin/Order/Details")]
        [HttpGet]
        public async Task<IActionResult> Details(string orderid, string productid, string optionvalue)
        {
            var orderreportview = _dataContext.OrderReportViews.FirstOrDefault(m => m.PayStatus == 2 && m.Type != 1 && m.OrderID == orderid && m.ProductID == productid && m.ProductOptionValue == optionvalue);

            return View(orderreportview);
        }
        [Route("/admin/Order/Details")]
        [HttpPost]
        public async Task<IActionResult> Details(string orderid, string productid, string optionvalue, int set)
        {
            var orderdetail = _dataContext.OrderDetails.FirstOrDefault(m=> m.ProductID == productid && m.OrderID == orderid && m.ProductOptionValue == optionvalue);
            var orderreportview = _dataContext.OrderReportViews.FirstOrDefault(m => m.PayStatus == 2 && m.Type != 1 && m.OrderID == orderid && m.ProductID == productid && m.ProductOptionValue == optionvalue);
            if (set == 1 && orderreportview != null)
            {
                orderdetail.OrderStatusID = 3;
                _dataContext.Update(orderdetail);
                _dataContext.SaveChanges();
            }
            return View(orderreportview);
        }
        [Route("/admin/Order/History")]
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var list_order = _dataContext.OrderReportViews.Where(m => m.PayStatus == 2 && m.Type != 1 && m.OrderStatusID == 3).ToList();

            return View(list_order);
        }
    }
}

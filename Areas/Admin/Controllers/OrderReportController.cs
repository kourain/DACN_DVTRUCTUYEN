using DACN_DVTRUCTUYEN.Areas.User.Models;
using DACN_DVTRUCTUYEN.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Mvc;
using DACN_DVTRUCTUYEN.Areas.Admin.Models;
namespace DACN_DVTRUCTUYEN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderReportController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderReportController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("/admin/OrderReport")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Redirect("/admin/order/Come");
        }
        [Route("/admin/OrderReport/Come")]
        [HttpGet]
        public async Task<IActionResult> Come()
        {
            var keys = _dataContext.Product_Keys.Where(m=> m.Rp_FromUser_Time != null && m.Rp_Response_Time == null).ToList();
            List<(Product_Key,OrderReportView)> result = new List<(Product_Key, OrderReportView)>();
            foreach(var item in keys)
            {
                result.Add((item,_dataContext.OrderReportViews.FirstOrDefault(m=> m.OrderID == item.OrderID && m.ProductID == item.ProductID && m.ProductOptionValue == item.OptionValue)));
            }
            return View(result);
        }
        [Route("/admin/OrderReport/processing")]
        [HttpGet]
        public async Task<IActionResult> Processing()
        {
            var list_order = _dataContext.OrderReportViews.Where(m => m.PayStatus == 2 && m.Type != 1 && m.OrderStatusID == 2).ToList();

            return View(list_order);
        }
        [Route("/admin/OrderReport/Details")]
        [HttpGet]
        public async Task<IActionResult> Details(string orderid, string productid, string optionvalue,string key1)
        {
            var keys = _dataContext.Product_Keys.FirstOrDefault(m => m.OrderID == orderid && m.ProductID == productid && m.OptionValue == optionvalue && m.Key1 == key1);
            if(key1 == null) { return NotFound(); }
            return View((keys, _dataContext.OrderReportViews.FirstOrDefault(m => m.OrderID == keys.OrderID && m.ProductID == keys.ProductID && m.ProductOptionValue == keys.OptionValue)));
        }
        [Route("/admin/OrderReport/Details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(string orderid, string productid, string optionvalue, string key1,string rp_response, int set)
        {
            var keys = _dataContext.Product_Keys.FirstOrDefault(m => m.OrderID == orderid && m.ProductID == productid && m.OptionValue == optionvalue && m.Key1 == key1);
            var orderreportview = _dataContext.OrderReportViews.FirstOrDefault(m => m.OrderID == keys.OrderID && m.ProductID == keys.ProductID && m.ProductOptionValue == keys.OptionValue);
            if (keys != null && !string.IsNullOrEmpty(rp_response))
            {
                string submess = $"\n";
                keys.Rp_Response = rp_response;
                keys.Rp_Response_Time = DateTime.Now;
                if (set == 1 && keys.Rp_SendNew == null) {
                    keys.Rp_SendNew = true;
                    submess += $"kèm 1 tài khoản mới";
                    var orderdetail = _dataContext.OrderDetailViews.FirstOrDefault(m => m.ProductID == productid && m.OrderID == orderid && m.ProductOptionValue == optionvalue);
                    Areas.User.Services.OrderBackgroundService.EnqueueRequest((orderdetail, _dataContext.Users.FirstOrDefault(m=> m.UserId == orderreportview.UserID)),true);
                }
                else if(keys.Rp_Response == null)
                {
                    keys.Rp_SendNew = false;
                }
                _dataContext.Update(keys);
                _dataContext.SaveChanges();
                TelegramBot.Controllers.HomeController.SendMess(_dataContext.OrderReportViews.FirstOrDefault(m => m.OrderID == orderid).TelegramChatID,
                    $"DB Shop phản hồi cho đơn hàng \"{keys.OrderID}\":" +
                    $"\nphản hồi của bạn: \"{keys.Rp_FromUser}\" \nvào lúc {keys.Rp_FromUser_Time?.ToString("HH:mm:ss dd/MM/yyyy")}" +
                    $"\nChúng tôi: \"{keys.Rp_Response}\" \nvào lúc {keys.Rp_Response_Time?.ToString("HH:mm:ss dd/MM/yyyy")}" + submess);
            }
            return View((keys,orderreportview));
        }
        [Route("/admin/OrderReport/History")]
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var keys = _dataContext.Product_Keys.Where(m => m.Rp_FromUser_Time != null && m.Rp_Response_Time != null).ToList();
            List<(Product_Key, OrderReportView)> result = new List<(Product_Key, OrderReportView)>();
            foreach (var item in keys)
            {
                result.Add((item, _dataContext.OrderReportViews.FirstOrDefault(m => m.OrderID == item.OrderID && m.ProductID == item.ProductID && m.ProductOptionValue == item.OptionValue)));
            }
            return View(result);
        }
    }
}

using DACN_DVTRUCTUYEN.Areas.User.Models;
using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;

namespace DACN_DVTRUCTUYEN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportController : Controller
    {
        private readonly DataContext _dataContext;
        public ReportController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("/admin/report")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Redirect("/admin/report/user");
        }
        [Route("/admin/report/user")]
        [HttpGet]
        public async Task<IActionResult> User(int date = 30)
        {
            var list_order = _dataContext.Orders.Where(m => m.PayStatus == 2 && m.Time >= DateTime.Now.AddDays(-date)).GroupBy(n => n.UserID).ToList();
            List<(Areas.User.Models.User?, int, long)> result = new List<(User.Models.User?, int, long)>();
            foreach (var item in list_order)
            {
                var list_this = item.ToList();
                int this_count = list_this.Count;
                long totalpaid = 0;
                foreach (var item2 in list_this)
                {
                    totalpaid += item2.TotalPay;
                }
                var this_result = (_dataContext.Users.FirstOrDefault(m => m.UserId == list_this[0].UserID), this_count, totalpaid);
                result.Add(this_result);
            }
            result = result.OrderByDescending(m => m.Item3).Take(20).ToList();
            return View((result, date));
        }
        [Route("/admin/report/productoption")]
        [HttpGet]
        public async Task<IActionResult> productoption(int date = 30)
        {
            var list_order = _dataContext.OrderViews.Where(m => m.PayStatus == 2 && m.Time >= DateTime.Now.AddDays(-date)).GroupBy(n => n.ProductID).ToList();
            List<(ProductView?, int, int, long)> result = new List<(ProductView?, int, int, long)>();
            foreach (var item in list_order)
            {
                var list_this = item.ToList();
                int tongsldon = list_this.Count;
                int sldon = 0;
                long slspban = 0;
                string optionvalue = "";

                foreach (var item2 in list_this.GroupBy(m => m.ProductOptionValue).ToList())
                {
                    if (sldon < item2.Count())
                    {
                        sldon = item2.Count();
                        optionvalue = item2.Key;
                    }
                    slspban+= item2.Sum(m => m.Quantity);
                }
                var this_result = (_dataContext.ProductViews.FirstOrDefault(m => m.ProductID == list_this[0].ProductID && m.OptionValue == optionvalue), sldon, tongsldon, slspban);
                result.Add(this_result);
            }
            result = result.OrderByDescending(m => m.Item4).Take(20).ToList();
            return View((result, date));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using DACN_DVTRUCTUYEN.Models;
using DACN_DVTRUCTUYEN.Areas.User.Models;
using System.Collections.Generic;
using System.Net.WebSockets;
using Microsoft.EntityFrameworkCore;

namespace DACN_DVTRUCTUYEN.Areas.Admin.Components
{
    [ViewComponent(Name = "SaleCard")]
    public class SaleCardComponent : ViewComponent
    {
        private readonly DataContext _context;
        public SaleCardComponent(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var thismonth = _context.Orders.FromSql(System.Runtime.CompilerServices.FormattableStringFactory.Create($"SELECT * FROM [DBO].[ORDER] WHERE MONTH(TIME) = {DateTime.Now.Month} AND YEAR(TIME) = {DateTime.Now.Year} AND PAYSTATUS=2 OR PAYSTATUS=1"));
            var lastmonth = _context.Orders.FromSql(System.Runtime.CompilerServices.FormattableStringFactory.Create($"SELECT * FROM [DBO].[ORDER] WHERE MONTH(TIME) = {DateTime.Now.AddMonths(-1).Month} AND YEAR(TIME) = {DateTime.Now.AddMonths(-1).Year} AND PAYSTATUS=2 OR PAYSTATUS=1"));
            var total_thismonth = thismonth.Sum(m => m.TotalPay);
            var total_lastmonth = lastmonth.Sum(m => m.TotalPay);
            //iqueriable join, tuple
            ((IList<OrderView>, int), (IList<OrderView>, int), (string, string)) tuple = ((thismonth.Join(_context.OrderDetails, m => m.OrderID, n => n.OrderID, (m, n) => new OrderView()
            {
                OrderID = m.OrderID,
                ProductID = n.ProductID,
            }).ToList(), total_thismonth), (lastmonth.Join(_context.OrderDetails, m => m.OrderID, n => n.OrderID, (m, n) => new OrderView()
            {
                OrderID = m.OrderID,
                ProductID = n.ProductID,
            }).ToList(), total_lastmonth), ("", ""));
            //lấy product được mua nhiều nhất tháng này
            OrderView? item = tuple.Item1.Item1
                            .GroupBy(n => n)
                            .OrderByDescending(g => g.Count())
                            .Select(g => g.Key)
                            .FirstOrDefault();
            if(item != null)
            {

                tuple.Item3.Item1 = item.ProductID;
            }
            //lấy product được mua nhiều nhất tháng trước
            OrderView? item2 = tuple.Item2.Item1
                            .GroupBy(n => n)
                            .OrderByDescending(g => g.Count())
                            .Select(g => g.Key)
                            .FirstOrDefault();
            if (item2 != null)
            {

                tuple.Item3.Item2 = item2.ProductID;
            }
            return await Task.FromResult((IViewComponentResult)View("Default", tuple));
        }
    }
}

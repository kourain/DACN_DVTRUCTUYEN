﻿using DACN_DVTRUCTUYEN.Areas.User.Models;
using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACN_DVTRUCTUYEN.Areas.User.Components
{
    [ViewComponent(Name = "NewProduct")]
    public class NewProductComponent : ViewComponent
    {
        private readonly DataContext _context;
        public NewProductComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //iqueriable join
            var list = _context.Products.OrderByDescending(m => m.CreateDate).Take(8).Join(_context.ProductOptions, m => m.ProductID, n => n.ProductID, (m, n) => new ProductView()
            {
                ProductID = m.ProductID,
                ProductName = m.ProductName,
                ProductImg = m.ProductImg,
                OptionName = n.OptionName,
                OptionValue = n.OptionValue,
                ProductDescription = m.ProductDescription,
                PriceOld = n.PriceOld,
                PriceNow = n.PriceNow,
            }).GroupBy(p => p.ProductID).Select(g => g.First()).ToList(); // loại bỏ các product trùng nhau
            return await Task.FromResult((IViewComponentResult)View("Default", list));
        }
    }
}

using DACN_DVTRUCTUYEN.Areas.User.Models;
using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACN_DVTRUCTUYEN.Areas.User.Components
{
    [ViewComponent(Name = "Carousel")]
    public class CarouselComponent : ViewComponent
    {
        private readonly DataContext _context;
        public CarouselComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //iqueriable join
            var list = _context.Products.OrderByDescending(m => m.CreateDate).Take(4).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", list));
        }
    }
}

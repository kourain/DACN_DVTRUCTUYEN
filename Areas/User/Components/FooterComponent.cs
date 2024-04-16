using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;

namespace DACN_DVTRUCTUYEN.Areas.User.Components
{
    [ViewComponent(Name = "Footer")]
    public class FooterComponent : ViewComponent
    {
        private readonly DataContext _context;
        public FooterComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var list = _context.Footers.Where(m => m.IsActive == true).ToList() ;
            return await Task.FromResult((IViewComponentResult)View("Default",list));
        }
    }
}

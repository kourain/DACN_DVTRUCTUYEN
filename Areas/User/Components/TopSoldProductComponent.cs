using DACN_DVTRUCTUYEN.Areas.User.Models;
using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;

namespace DACN_DVTRUCTUYEN.Areas.User.Components
{
    [ViewComponent(Name = "TopSoldProduct")]
    public class TopSoldProductComponent : ViewComponent
    {
        private readonly DataContext _context;
        public TopSoldProductComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("Default"));
        }
    }
}

using DACN_DVTRUCTUYEN.Areas.User.Models;
using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;

namespace DACN_DVTRUCTUYEN.Areas.User.Components
{
    [ViewComponent(Name = "TopProduct")]
    public class TopProductComponent : ViewComponent
    {
        private readonly DataContext _context;
        public TopProductComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult((IViewComponentResult)View("Default"));
        }
    }
}

using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;

namespace DACN_DVTRUCTUYEN.Areas.User.Components
{
    [ViewComponent(Name = "UserHeadMenuView")]
    public class UserHeadMenuViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        public UserHeadMenuViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listofMenu = "";// (from m in _context.Menus where (m.IsActive == true) select m).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default"));
            //return await Task.FromResult((IViewComponentResult)View("Default"));
        }
    }
}

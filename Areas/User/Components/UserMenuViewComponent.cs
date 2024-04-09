using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;

namespace DACN_DVTRUCTUYEN.Areas.User.Components
{
    [ViewComponent(Name = "UserMenuView")]
    public class UserMenuViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        public UserMenuViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listofMenu = (from m in _context.UserMenus where (m.IsActive == true) select m).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default",listofMenu));
        }
    }
}

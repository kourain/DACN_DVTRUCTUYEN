using DACN_DVTRUCTUYEN.Models;
using DACN_DVTRUCTUYEN.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DACN_DVTRUCTUYEN.Areas.User.Components
{
    [ViewComponent(Name = "CheckBan")]
    public class CheckBanComponent : ViewComponent
    {
        private readonly DataContext _context;
        public CheckBanComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return await Task.FromResult((IViewComponentResult)View("Default", 1));
            }
            var us = _context.Users.Where(m => m.UserId == userid).FirstOrDefault();
            if (us.Ban == true)
            {
                return await Task.FromResult((IViewComponentResult)View("Default", 1));
            }
            return await Task.FromResult((IViewComponentResult)View("Default", 0));
        }
    }
}

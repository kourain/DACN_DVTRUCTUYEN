using Microsoft.AspNetCore.Mvc;
using DACN_DVTRUCTUYEN.Models;
using DACN_DVTRUCTUYEN.Utilities;
using Microsoft.AspNetCore.Hosting.Server;
using System;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Cryptography;

namespace DACN_DVTRUCTUYEN.Controllers
{
    public class AuthenticatorController : Controller
    {
        public AuthenticatorController(DataContext dataContext)
        {

        }
        [Route("/User/Logout")]
        public ActionResult UserLogout()
        {
            //xóa cookie
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return Redirect("/User/Login");
        }
        [Route("/Admin/Logout")]
        public ActionResult AdminLogout()
        {
            //xóa cookie
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return Redirect("/Admin/Login");
        }
    }
}

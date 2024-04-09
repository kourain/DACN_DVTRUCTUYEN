using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;
using DACN_DVTRUCTUYEN.Utilities;
using System.Net;
using System.Web;
namespace DACN_DVTRUCTUYEN.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        public HomeController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("/User")]
        public IActionResult Index()
        {
            return View();
        }

    }
}

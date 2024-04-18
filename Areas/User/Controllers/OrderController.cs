using Microsoft.AspNetCore.Mvc;

namespace DACN_DVTRUCTUYEN.Areas.User.Controllers
{
    [Area("User")]
    public class OrderController : Controller
    {
        [Route("/user/Order/{orderid:int}&{vnp_transid}&{orderinfor}")]
        public IActionResult Index(int orderid,string vnp_transid, string orderinfor)
        {
            return Ok(orderid + "\n" + vnp_transid + "\n" + orderinfor + "\n");
            return View();
        }
    }
}

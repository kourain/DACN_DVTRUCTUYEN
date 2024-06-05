using Microsoft.AspNetCore.Mvc;
using System;
using System.Configuration;
using System.Web;
using DACN_DVTRUCTUYEN.Areas.VNPayAPI.Util;
using DACN_DVTRUCTUYEN.Areas.User.Models;
using DACN_DVTRUCTUYEN.Models;

namespace DACN_DVTRUCTUYEN.Areas.VNPayAPI.Controllers
{
    [Area("VNPayAPI")]
    public class HomeController : Controller
    {
        public static string url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public static string returnUrl = "https://localhost:44311/vnpayAPI/PaymentConfirm";
        public static string tmnCode = "W0NBIFXR";
        public static string hashSecret = "RNOIJLQLRSXPYXUPKHXBQMGJZAWJAVPV";
        private readonly DataContext _dataContext;
        public HomeController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public ActionResult Index()
        {
            return View();
        }
        public static string Payment(string amount, string infor, string orderinfor)
        {
            string hostName = System.Net.Dns.GetHostName();
            string clientIPAddress = System.Net.Dns.GetHostAddresses(hostName).GetValue(0).ToString();
            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", amount); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", clientIPAddress); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", infor); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", orderinfor); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            return paymentUrl;
        }
        [Route("/VNpayAPI/paymentconfirm")]
        public async Task<IActionResult> PaymentConfirm()
        {
            if (Request.QueryString.HasValue)
            {
                //lấy toàn bộ dữ liệu trả về
                var queryString = Request.QueryString.Value;
                var json = HttpUtility.ParseQueryString(queryString);

                string orderId = json["vnp_TxnRef"].ToString(); //mã hóa đơn
                string orderInfor = json["vnp_OrderInfo"].ToString(); //Thông tin giao dịch
                long vnpayTranId = Convert.ToInt64(json["vnp_TransactionNo"]); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = json["vnp_ResponseCode"].ToString(); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = json["vnp_SecureHash"].ToString(); //hash của dữ liệu trả về
                var pos = Request.QueryString.Value.IndexOf("&vnp_SecureHash");

                //return Ok(Request.QueryString.Value.Substring(1, pos-1) + "\n" + vnp_SecureHash + "\n"+ PayLib.HmacSHA512(hashSecret, Request.QueryString.Value.Substring(1, pos-1)));
                bool checkSignature = ValidateSignature(Request.QueryString.Value.Substring(1, pos - 1), vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?
                if (checkSignature && tmnCode == json["vnp_TmnCode"].ToString())
                {
                    var value = _dataContext.Orders.Where(m => m.OrderID == orderId && m.PayStatus == 0).FirstOrDefault();
                    if (value == null)
                        return Redirect("/user/");
                    var user = _dataContext.Users.Where(m => m.UserId == value.UserID).FirstOrDefault();
                    if (user == null)
                        return Redirect("/user/");
                    if (vnp_ResponseCode == "00")
                    {
                        //send mess
                        TelegramBot.Controllers.HomeController.SendMess(user.TelegramChatID, $"Chào {user.Name}\n\tBạn vừa thanh toán cho đơn hàng {orderId}");
                        value.PayStatus = 1;
                        value.TransactionNo = vnpayTranId;
                        _dataContext.Update(value);
                        _dataContext.SaveChanges();
                        //Thanh toán thành công
                        return Redirect($"/user/Orders/OK/{orderId}&{vnpayTranId}&{orderInfor}");
                    }
                    else
                    {
                        TelegramBot.Controllers.HomeController.SendMess(_dataContext.Users.FirstOrDefault(m => m.UserId == value.UserID).TelegramChatID, $"Thanh toán thất bại tại đơn hàng {value.OrderID}");
                        value.PayStatus = -1;
                        value.TransactionNo = vnpayTranId;
                        _dataContext.Update(value);
                        _dataContext.SaveChanges();
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        return Redirect($"/user/Orders/ERROR/{orderId}&{vnpayTranId}&{orderInfor}");
                    }
                }
                else
                {
                    //phản hồi không khớp với chữ ký
                    return BadRequest();
                }
            }
            //phản hồi không hợp lệ
            return BadRequest();
        }
        public bool ValidateSignature(string rspraw, string inputHash, string secretKey)
        {
            string myChecksum = PayLib.HmacSHA512(secretKey, rspraw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
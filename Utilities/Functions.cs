using DACN_DVTRUCTUYEN.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System;
using System.Security.AccessControl;
using System.Web;
using Newtonsoft.Json.Linq;
namespace DACN_DVTRUCTUYEN.Utilities
{
    public class Functions
    {
        protected static JsonObject tokenUser = new JsonObject();
        protected static JsonObject tokenAdmin = new JsonObject();
        public static string MD5Hash(string text)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(text);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder strBuilder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    strBuilder.Append(hashBytes[i].ToString("x2"));
                }
                return strBuilder.ToString();
            }
        }
        public static void saveLoginUser(string agg1, string agg2,string agg3, bool agg4)
        {

            var thisval = tokenUser[agg1];
            //lưu token tại hệ thống với thời hạn sử dụng là 6 giờ
            try { 
                tokenUser.Add(agg1, JsonNode.Parse(JsonSerializer.Serialize(new
                {
                    //6 giờ cho thời hạn token, 7 giờ do hệ thống đang sử dụng giờ GMT+7
                    time = DateTime.Now.AddHours(6 + 7).ToString("yyyy-MM-dd HH:mm:ss"),
                    UID = agg2,
                    email = agg3
                })));
            }
            catch { 
                //6 giờ cho thời hạn token, 7 giờ do hệ thống đang sử dụng giờ GMT+7
                tokenUser[agg1]["time"] = DateTime.Now.AddHours(6 + 7).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (agg4 == true)
                tokenUser[agg1]["time"] = "1";
        }
        public static void LogoutUser(string agg1)
        {
            if (string.IsNullOrEmpty(agg1)) return;
            //đặt lại quá hạn cho thời hạn sử dụng token có trên hệ thống
            var thisval = tokenUser[agg1];
            if (thisval != null)
                tokenUser[agg1]["time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static int GetUIDFromToken(string? posttokenUser)
        {
            if (string.IsNullOrEmpty(posttokenUser)) return 0;
            var thisval = tokenUser[posttokenUser];
            if (thisval == null)
                return 0;
            return int.Parse(thisval["UID"].ToString());
            
        }

        public static int IsLoginUser(string? posttokenUser, string? uid = "")
        {
            if (string.IsNullOrEmpty(posttokenUser)) return 0;
            var thisval = tokenUser[posttokenUser];
            if (thisval == null)
                return 0;
            if ((string)thisval["UID"] != uid)
                return 0;
            if ((string)thisval["time"] != "1")
                if (DateTime.Compare(DateTime.Now, DateTime.Parse((string)thisval["time"])) > 0) { return 0; }
            return 1;
        }
        public static void tokenChangeUser(string oldkey, string newkey)
        {
            if (string.IsNullOrEmpty(oldkey) || string.IsNullOrEmpty(newkey)) return;// Rename the key
            tokenUser[newkey] = tokenUser[oldkey];
            tokenUser.Remove(oldkey);
        }
        public static void saveLoginAdmin(string agg1, string agg2, int agg3)
        {
            var thisval = tokenAdmin[agg1];
            //lưu token tại hệ thống với thời hạn sử dụng là 6 giờ
            if (thisval == null)
                tokenAdmin.Add(agg1, JsonNode.Parse(JsonSerializer.Serialize(new
                {
                    adminid = agg3,
                    //6 giờ cho thời hạn token, 7 giờ do hệ thống đang sử dụng giờ GMT+7
                    time = DateTime.Now.AddHours(6 + 7).ToString("yyyy-MM-dd HH:mm:ss"),
                    UID = agg2
                })));
            else
            {
                //6 giờ cho thời hạn token, 7 giờ do hệ thống đang sử dụng giờ GMT+7
                tokenAdmin[agg1]["time"] = DateTime.Now.AddHours(6 + 7).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public static void LogoutAdmin(string agg1)
        {
            if (string.IsNullOrEmpty(agg1)) return;
            //đặt lại quá hạn cho thời hạn sử dụng token có trên hệ thống
            var thisval = tokenAdmin[agg1];
            if (thisval != null)
                tokenAdmin[agg1]["time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static int IsLoginAdmin(string? posttokenAdmin, string? uid = "")
        {

            if (string.IsNullOrEmpty(posttokenAdmin)) return 0;
            var thisval = tokenAdmin[posttokenAdmin];
            if (thisval == null)
                return 0;
            if (thisval["UID"].ToString().ToLower() != HttpUtility.UrlDecode(uid).ToLower())
                return 0;
            if (DateTime.Compare(DateTime.Now, DateTime.Parse((string)thisval["time"])) > 0) { return 0; }
            return (int)thisval["adminid"];
        }
        public static void tokenChangeAdmin(string oldkey, string newkey)
        {
            if (string.IsNullOrEmpty(oldkey) || string.IsNullOrEmpty(newkey)) return;// Rename the key
            tokenAdmin[newkey] = tokenAdmin[oldkey];
            tokenAdmin.Remove(oldkey);
        }
    }
}
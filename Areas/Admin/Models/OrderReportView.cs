using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Areas.Admin.Models
{
    [Table("OrderReportView")]
    [PrimaryKey(nameof(OrderID), nameof(ProductID), nameof(ProductOptionValue))]
    public class OrderReportView
    {
        [Key]
        //order
        public string OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime Time { get; set; }
        public int TotalPay { get; set; }
        public Int16 PayStatus { get; set; }
        public long? TransactionNo { get; set; }
        public string? Note { get; set; }
        //order Detail
        public string ProductID { get; set; }
        public string ProductOptionValue { get; set; }
        public Int16 OrderStatusID { get; set; }
        public long Amount { get; set; }
        public int Quantity { get; set; }
        //product
        public string? ProductName { get; set; }
        //product option
        public string OptionName { get; set; }
        public byte Type { get; set; }
        //user
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public long? TelegramChatID { get; set; }
        public string? TelegramName { get; set; }
        public string? TelegramUserName { get; set; }
    }
}
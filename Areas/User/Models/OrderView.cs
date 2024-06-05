using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Areas.User.Models
{
    [Table("OrderView")]
    [PrimaryKey(nameof(OrderID),nameof(ProductID), nameof(ProductOptionValue))]
    public class OrderView
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
    }
}
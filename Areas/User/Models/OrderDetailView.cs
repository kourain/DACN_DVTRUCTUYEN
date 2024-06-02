using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Areas.User.Models
{
    [Table("OrderDetailView")]
    [PrimaryKey(nameof(OrderID), nameof(ProductID), nameof(ProductOptionValue))]
    public class OrderDetailView
    {
        [Key]
        //order
        public string OrderID { get; set; }
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
        public int Type { get; set; }
    }
}
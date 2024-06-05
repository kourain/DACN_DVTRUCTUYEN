using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Areas.User.Models
{
    [Table("CartView")]
    [PrimaryKey(nameof(UserID),nameof(ProductID),nameof(OptionValue))]
    public class CartView
    {
        //cart
        [Key, Column(Order = 0)]
        public int UserID { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateDate { get; set; }
        //Product - Key: ProductID
        [Key,Column(Order =1)]
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string? ProductImg { get; set; }
        //public int ViewCount { get; set; }
        //ProductOption - Key: OptionValue
        [Key, Column(Order = 2)]
        public string OptionValue { get; set; }
        public string OptionName { get; set; }
        public int ProductOptionQuantity { get; set; }
        public int PriceOld { get; set; }
        public int PriceNow { get; set; }
        public byte Type { get; set; }
    }
}

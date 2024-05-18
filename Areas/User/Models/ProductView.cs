using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Areas.User.Models
{
    [Table("ProductView")]
    [PrimaryKey(nameof(ProductID),nameof(OptionValue))]
    public class ProductView
    {
        //Product - Key: ProductID
        [Key,Column(Order =0)]
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public DateTime ProductCreateDate { get; set; }
        public string? ProductImg { get; set; }
        public int ViewCount { get; set; }
        //ProductOption - Key: OptionValue
        [Key, Column(Order = 1)]
        public string OptionValue { get; set; }
        public string OptionName { get; set; }
        public int Quantity { get; set; }
        public int SoldCount { get; set; }
        public int PriceOld { get; set; }
        public int PriceNow { get; set; }
        public int Type { get; set; }
        public DateTime? OptionCreateDate { get; set; }
    }
}

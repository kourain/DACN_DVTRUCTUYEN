using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Models
{
    [Table("ProductOption")]
    [PrimaryKey(nameof(ProductID), nameof(OptionValue))]
    public class ProductOption
    {
        [Key]
        public string ProductID { get; set; }
        public string OptionValue { get; set; }
        public string OptionName { get; set; }
        public int Quantity { get; set; }
        public int SoldCount { get; set; }
        public int PriceOld { get; set; }
        public int PriceNow { get; set; }
        public int Type { get; set; }
        public DateTime? CreateDate { get; set; }
        public ProductOption() { CreateDate = DateTime.Now; }
    }
}
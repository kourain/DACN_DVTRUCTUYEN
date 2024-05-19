using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Areas.User.Models
{
    [Table("Cart")]
    [PrimaryKey(nameof(UserID), nameof(ProductID), nameof(ProductOptionValue))]
    public class Cart
    {
        public Cart() { CreateDate = DateTime.Now; }
        [Key]
        public int UserID { get; set; }
        public string ProductID { get; set; }
        public string ProductOptionValue { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
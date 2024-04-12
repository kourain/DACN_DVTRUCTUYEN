using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Models
{
    [Table("Product")]
    public class Product
    {
        public Product() { CreateDate = DateTime.Now; }
        [Key]
        public string ProductID { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public DateTime CreateDate { get; set; }
        public string? ProductImg { get; set; }
    }
}
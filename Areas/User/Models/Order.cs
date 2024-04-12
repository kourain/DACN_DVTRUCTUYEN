using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Areas.User.Models
{
    [Table("Order")]
    public class Order
    {
        public Order() { Time = DateTime.Now; }
        [Key]
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public int TotalPay { get; set; }
        public DateTime Time { get; set; }
    }
}
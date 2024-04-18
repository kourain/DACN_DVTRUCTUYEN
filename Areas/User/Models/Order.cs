using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Areas.User.Models
{
    [Table("Order")]
    public class Order
    {
        public Order() { Time = DateTime.Now; }
        [Key]
        public long OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime Time { get; set; }
        public int TotalPay { get; set; }
        public int PayStatus { get; set; }
    }
}
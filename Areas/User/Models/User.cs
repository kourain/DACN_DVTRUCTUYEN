using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Areas.User.Models
{
    [Table("User")]
    public class User
    {
        public User() { CreateDate = DateTime.Now; }
        [Key]
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public bool? Ban { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DACN_DVTRUCTUYEN.Areas.Admin.Models
{
	[Table("AdminUser")]
	public class AdminUser
	{
		[Key]
        public int AdminUserID { get; set; }
        public string AdminName { get; set; }
		public string? PassWord { get; set; }
		public string? Email { get; set; }
	}
}

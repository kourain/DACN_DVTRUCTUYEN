using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Areas.User.Models
{
    [Table("UserMenu")]
    public class UserMenu
    {
        [Key]
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public bool IsActive { get; set; }
        public string ControllerName { get; set; }
        public string? ActionName { get; set; }
        public int ParentID { get; set; }
        public string? Link { get; set; }
        public int MenuOrder { get; set; }
        public bool? IsDropDownItem { get; set; }
        public string? Icon { get; set; }
    }
}

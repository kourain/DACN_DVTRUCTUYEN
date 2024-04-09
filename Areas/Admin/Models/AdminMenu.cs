using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Areas.Admin.Models
{
    [Table("AdminMenu")]
    public class AdminMenu

    {
        [Key]
        public int AdminMenuId { get; set; }
        public string ItemName { get; set; }
        public int ParentLevel { get; set; }
        public int ItemOrder { get; set; }
        public bool IsActive { get; set; }
        public string? ItemDropDown { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string? Icon { get; set; }
    }
}

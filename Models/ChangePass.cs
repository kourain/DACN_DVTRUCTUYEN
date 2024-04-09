using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Models
{
    public class ChangePass
    {
        public string oldPass { get; set; }
        public string newPass { get; set; }
        public string renewPass { get; set; }
    }
}
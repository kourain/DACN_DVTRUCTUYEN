using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DACN_DVTRUCTUYEN.Models
{
    [Table("Product_Question")]
    public class Product_Question
    {
        [Key]
        public int QuestionId { get; set; }
        public string ProductID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
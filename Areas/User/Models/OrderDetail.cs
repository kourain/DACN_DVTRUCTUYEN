﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Areas.User.Models
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key]
        public int OrderID { get; set; }
        public string ProductID { get; set; }
        public string ProductOptionValue { get; set; }
        public int OrderStatus { get; set; }
    }
}
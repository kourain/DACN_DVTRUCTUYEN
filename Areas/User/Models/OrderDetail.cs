﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Areas.User.Models
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key]
        public string OrderID { get; set; }
        public string ProductID { get; set; }
        public string ProductOptionValue { get; set; }
        public int OrderStatusID { get; set; }
        public long Amount { get; set; }
    }
}
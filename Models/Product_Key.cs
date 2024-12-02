﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Models
{
    [Table("Product_Key")]
    [PrimaryKey(nameof(ProductID), nameof(OptionValue),nameof(Key1))]
    public class Product_Key
    {
        [Key]
        public string ProductID { get; set; }
        public string OptionValue { get; set; }
        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public string? OrderID { get; set; }
        public string? Rp_FromUser { get; set; }
        public DateTime? Rp_FromUser_Time { get; set; }
        public string? Rp_Response { get; set; }
        public DateTime? Rp_Response_Time { get; set; }
        public bool? Rp_SendNew { get; set; }
        public bool? From_SendNew { get; set; }
    }
}
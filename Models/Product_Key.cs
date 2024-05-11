﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACN_DVTRUCTUYEN.Models
{
    [Table("Product_Key")]
    [PrimaryKey(nameof(ProductID), nameof(OptionValue),nameof(Key))]
    public class Product_Key
    {
        [Key]
        public string ProductID { get; set; }
        public string OptionValue { get; set; }
        public string Key { get; set; }
        public bool IsSold { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PublicBicycles.Models
{
    public class CarOwner:IDbModel
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "用户名")]
        [StringLength(50)]
        [Required]
        public string Username { get; set; }

        [Display(Name = "密码")]
        [StringLength(50)]
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// 比如车位买断制
        /// </summary>
        [Display(Name = "免费用户")]
        [Required]
        public bool IsFree { get; set; } = false;
        [Display(Name = "是否启用")]
        [Required]
        public bool Enabled { get; set; } = true;
        [Display(Name ="注册时间")]
        public DateTime RegistTime { get; set; }
        [Display(Name ="上次登录时间")]
        public DateTime LastLoginTime { get; set; }
        public List<Car> Cars { get; set; }

        public List<TransactionRecord> TransactionRecords { get; set; }
    }
   
}

﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublicBicycles.Models
{
    /// <summary>
    /// 停车区
    /// </summary>
    public class PublicBicyclesArea : IDbModel
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "名称")]
        [StringLength(50)]
        [Required]
        public virtual string Name { get; set; }

        [Display(Name = "价格策略")]
        public int? PriceStrategyID { get; set; }
        public PriceStrategy PriceStrategy { get; set; }
        public virtual List<PublicBicyclesingSpace> PublicBicyclesingSpaces { get; set; }
        public virtual List<Aisle> Aisles { get; set; }
        public virtual List<Wall> Walls { get; set; }

        public virtual int Length { get; set; }
        public virtual int Width { get; set; }
        public string GateTokens { get; set; }="";

    }


}

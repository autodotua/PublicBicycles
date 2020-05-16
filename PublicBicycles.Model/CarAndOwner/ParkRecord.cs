using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublicBicycles.Models
{
    /// <summary>
    /// 停车记录
    /// </summary>
    public class PublicBicyclesRecord : IDbModel
    {
        [Key]
        public int ID { get; set; }

        public PublicBicyclesArea PublicBicyclesArea { get; set; }
        [Display(Name = "停车区")]
        [Required]
        public int PublicBicyclesAreaID { get; set; }

        [Display(Name = "车")]
        [Required]
        public Car Car { get; set; }
        [Display(Name = "车")]
        [Required]
        public int CarID { get; set; }
        [Display(Name = "进场时间")]
        [Required]
        public DateTime EnterTime { get; set; }
        [Display(Name = "离场时间")]
        [Required]
        public DateTime LeaveTime { get; set; }
        [Display(Name = "交易记录")]
        public TransactionRecord TransactionRecord { get; set; }


    }
}

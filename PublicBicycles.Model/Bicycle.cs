using System.ComponentModel.DataAnnotations;

namespace PublicBicycles.Models
{
    public class Bicycle : IDbModel
    {
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 自行车的物理ID，标牌上的ID
        /// </summary>
        public int BicycleID { get; set; }
        /// <summary>
        /// 停放的租赁点
        /// </summary>
        public Station Station { get; set; }
        /// <summary>
        /// 是否可以被借
        /// </summary>
        public bool CanHire { get; set; } = true;
        /// <summary>
        /// 是否已被删除
        /// </summary>
        public bool Deleted { get; set; } = false;
        /// <summary>
        /// 是否正在被借
        /// </summary>
        public bool Hiring { get; set; } = false;
    }

}

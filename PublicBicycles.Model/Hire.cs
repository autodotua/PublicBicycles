using System;
using System.ComponentModel.DataAnnotations;

namespace PublicBicycles.Models
{
    public class Hire : IDbModel
    {

        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 被借的时间
        /// </summary>
        public DateTime? HireTime { get; set; }
        /// <summary>
        /// 被借时停放的租赁点
        /// </summary>
        public Station HireStation { get; set; }
        /// <summary>
        /// 还车时间
        /// </summary>
        public DateTime? ReturnTime { get; set; }
        /// <summary>
        /// 还车的租赁点
        /// </summary>
        public Station ReturnStation { get; set; }
        /// <summary>
        /// 租借的用户
        /// </summary>
        public User Hirer { get; set; }
        /// <summary>
        /// 被借的自行车
        /// </summary>
        public Bicycle Bicycle { get; set; }
    }

}

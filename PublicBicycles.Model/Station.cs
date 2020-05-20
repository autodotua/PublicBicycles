using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PublicBicycles.Models
{
    public class Station : IDbModel
    {
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 可容纳的自行车数量（容量）
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 当前所拥有的自信车数量（这个其实是不符合数据库范式的，但是这样做能够减少数据库查询时间）
        /// </summary>
        public int BicycleCount { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Lng { get; set; }
        /// <summary>
        /// 是否可以归还
        /// </summary>
        public bool CanReturn { get; set; } = true;
        /// <summary>
        /// 是否在线（预留，无用）
        /// </summary>
        public bool Online { get; set; } = true;
        /// <summary>
        /// 是否已被删除
        /// </summary>
        public bool Deleted { get; set; }

    }

}

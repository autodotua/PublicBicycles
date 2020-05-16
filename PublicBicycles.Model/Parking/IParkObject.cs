using System;
using System.Collections.Generic;
using System.Text;

namespace PublicBicycles.Models
{
    public interface IPublicBicyclesObject:IDbModel
    {
        /// <summary>
        /// 分类，备用属性，暂时没用上
        /// </summary>
        public string Class { get; set; }
        public int PublicBicyclesAreaID { get; set; }
        public PublicBicyclesArea PublicBicyclesArea { get; set; }

    }
}

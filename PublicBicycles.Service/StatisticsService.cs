using Microsoft.EntityFrameworkCore;
using PublicBicycles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicBicycles.Service
{
    /// <summary>
    /// 统计服务
    /// </summary>
    public static class StatisticsService
    {
        /// <summary>
        /// 获取最近几天每一天的停车数量
        /// </summary>
        /// <param name="db"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public static async Task<IDictionary<DateTime, int>> GetRecentPublicBicyclesCount(PublicBicyclesContext db, int days)
        {
            //起始时间
            DateTime earliest = DateTime.Now.AddDays(-days);
            var records  =(await db.PublicBicyclesRecords
                    .Where(p => p.EnterTime > earliest).ToListAsync())
                    .GroupBy(p => p.EnterTime.Date);
            SortedDictionary<DateTime, int> result = new SortedDictionary<DateTime, int>();
            foreach (var day in records)
            {
                result.Add(day.Key, day.Count());
            }

            return result;
        }
    }
}

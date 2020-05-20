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
        /// 无用方法
        /// </summary>
        /// <param name="db"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public static async Task<IDictionary<DateTime, int>> GetRecentPublicBicyclesCount(PublicBicyclesContext db, int days)
        {
            //起始时间
            DateTime earliest = DateTime.Now.AddDays(-days);
            var records  =(await db.Hires
                    .Where(p => p.HireTime > earliest).ToListAsync())
                    .GroupBy(p => p.HireTime.Value.Date);
            SortedDictionary<DateTime, int> result = new SortedDictionary<DateTime, int>();
            foreach (var day in records)
            {
                result.Add(day.Key, day.Count());
            }

            return result;
        }

        /// <summary>
        /// 获取某一个站点最近几天借车和还车的相互联系的租赁点
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationID"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public static async Task<Route> GetStationRoutesAsync(PublicBicyclesContext db,int stationID,int days)
        {
            //起始时间
            DateTime earliest = DateTime.Now.AddDays(-days);
            List<Hire> ins = await db.Hires
                    .Where(p => p.ReturnTime > earliest)
                    .Where(p => p.ReturnStation.ID == stationID)
                    .Include(p => p.HireStation)
                    .ToListAsync();
            List<Hire> outs = await db.Hires
                    .Where(p => p.HireTime > earliest)
                    .Where(p => p.HireStation.ID == stationID)
                    .Where(p=>p.ReturnStation!=null)
                    .Include(p=>p.ReturnStation)
                    .ToListAsync();
            
            //<int,int>的前一个int是StationID，后一个int是来自/前往该租赁点的自行车数量
            Dictionary<int, int> outCount = outs.GroupBy(p => p.ReturnStation.ID).ToDictionary(p => p.Key, p => p.Count());
            Dictionary<int, int> intCount = ins.GroupBy(p => p.HireStation.ID).ToDictionary(p => p.Key, p => p.Count());
            return new Route()
            {
                In = intCount,
                Out = outCount,
            };
        }
    }
    public class Route
    {
        /// <summary>
        /// 入站（归还）的车辆的来源
        /// </summary>
        public Dictionary<int, int> In { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> Out { get; set; } = new Dictionary<int, int>();
    }
}

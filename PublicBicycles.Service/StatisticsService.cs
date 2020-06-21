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
            var records = (await db.Hires
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
        public static async Task<object> GetStationRoutesAsync(PublicBicyclesContext db, int stationID, int days)
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
                    .Where(p => p.ReturnStation != null)
                    .Include(p => p.ReturnStation)
                    .ToListAsync();

            //<int,int>的前一个int是StationID，后一个int是来自/前往该租赁点的自行车数量
            Dictionary<int, int> outCount = outs.GroupBy(p => p.ReturnStation.ID).ToDictionary(p => p.Key, p => p.Count());
            Dictionary<int, int> intCount = ins.GroupBy(p => p.HireStation.ID).ToDictionary(p => p.Key, p => p.Count());
            return new 
            {
                In = intCount,
                Out = outCount,
            };
        }

        public static async Task<object> GetLeaderboardAsync(PublicBicyclesContext db, int days)
        {
            //起始时间
            DateTime earliest = DateTime.Now.AddDays(-days);
            var hires = await db.Hires
                    .Where(p => p.ReturnTime > earliest)
                    .Include(p => p.HireStation)
                    .ToListAsync();
            var hireStations = hires
                .GroupBy(p => p.HireStation)
                .OrderByDescending(p => p.Count())
                .Take(10)
                .Select(p => new { p.Key.Name, Count = p.Count() })
                .ToList();
            var returnStations = hires
                .Where(p => p.ReturnStation != null)
                .GroupBy(p => p.ReturnStation)
                .OrderByDescending(p => p.Count())
                .Take(10)
                .Select(p => new { p.Key.Name, Count = p.Count() })
                .ToList();
            return new { hireStations, returnStations };
        }
        public static async Task<object> GetMoveNeededAsync(PublicBicyclesContext db)
        {
            var stations = await db.Stations.ToListAsync();
            var full = db.Stations
                .Where(p => 1.0 * p.BicycleCount / p.Count > 0.75)
                .Select(p => p.ID);
            var empty = db.Stations
                .Where(p => 1.0 * p.BicycleCount / p.Count < 0.25)
                .Select(p => p.ID);
            return new { full, empty };
        }
    }

}

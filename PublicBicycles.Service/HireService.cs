using Microsoft.EntityFrameworkCore;
using PublicBicycles.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PublicBicycles.Service
{
    /// <summary>
    /// 与借车相关的服务
    /// </summary>
    public static class HireService
    {
        /// <summary>
        /// 用于测试，获取当前时间
        /// </summary>
        internal static Func<DateTime> Now { get; set; } = () => DateTime.Now;
        /// <summary>
        /// 用于测试，指定是否需要自动将更改写入数据库
        /// </summary>
        internal static bool SaveChanges { get; set; } = true;
        //internal static System.Collections.Generic.List<Hire> Hires { get; set; }
        public static async Task<HireResult> HireAsync(PublicBicyclesContext db, int userID, int bicycleID, int stationID)
        {
            User user = db.Users.Find(userID);
            Station station = db.Stations.Find(stationID);
            Bicycle bicycle = db.Bicycles.Find(bicycleID);
            if (user == null || station == null || bicycle == null)
            {
                return new HireResult(null, HireResultType.DatabaseError);
            }
            ///获取最后一条没有完成的借车记录
            Hire hire = await db.Hires.LastOrDefaultRecordAsync(p => p.HireTime.Value, p => p.Hirer.ID == userID && p.ReturnStation == null);
            if (hire != null)
            {
                //如果发现用户还没有还车，那么不允许借车
                return new HireResult(null, HireResultType.AnotherIsHired);
            }
            hire = new Hire()
            {
                HireStation = station,
                HireTime = Now(),
                Bicycle = bicycle,
                Hirer = user,
            };
            db.Hires.Add(hire);
            station.BicycleCount--;
            db.Stations.Update(station);
            //更新信息，包括车站自行车-1，设置自行车的被借状态，设置自行车的租赁点为null
            bicycle.Hiring = true;
            bicycle.Station = null;
            db.Update(bicycle);
            if (SaveChanges)
            {
                await db.SaveChangesAsync();
            }
            return new HireResult(hire, HireResultType.Succeed);
        }
        /// <summary>
        /// 获取最后一条没有还车的借车记录
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static async Task<Hire> GetHiringAsync(PublicBicyclesContext db, int userID)
        {
            return await db.Hires
                .OrderByDescending(p => p.HireTime.Value)
                .Where(p => p.Hirer.ID == userID && p.ReturnStation == null)
                .Include(p => p.Bicycle)
                .Include(p => p.HireStation)
                .FirstOrDefaultAsync();
        }
        /// <summary>
        /// 还车
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userID"></param>
        /// <param name="bicycleID"></param>
        /// <param name="stationID"></param>
        /// <returns></returns>
        public static async Task<ReturnResult> ReturnAsync(PublicBicyclesContext db, int userID, int bicycleID, int stationID)
        {
            User user = db.Users.Find(userID);
            Station station = db.Stations.Find(stationID);
            Bicycle bicycle = db.Bicycles.Find(bicycleID);
            Hire hire = await GetHiringAsync(db, userID);
            //if (hire == null && Hires != null && Hires.Any(p => p.Hirer.ID == userID))
            //{
            //    hire = Hires.First(p => p.Hirer.ID == userID);
            //}
            if (user == null || station == null || bicycle == null || hire == null)
            {
                return new ReturnResult(null, ReturnResultType.DatabaseError);
            }
            if (hire == null)
            {
                //找不到借车记录，联系客服
                return new ReturnResult(null, ReturnResultType.RecordNotFound);
            }
            if (!station.CanReturn || !station.Online)
            {
                //表示租赁点不可还车或没有在线
                return new ReturnResult(hire, ReturnResultType.StationCannotReturn);
            }
            if (station.BicycleCount >= station.Count)
            {
                //表示租赁点已满
                return new ReturnResult(hire, ReturnResultType.StationIsFull);
            }
            hire.ReturnStation = station;
            hire.ReturnTime = Now();
            db.Hires.Update(hire);
            //更新信息，包括车站自行车+1，设置自行车的被借状态，设置自行车的新租赁点为
            station.BicycleCount++;
            db.Stations.Update(station);

            bicycle.Hiring = false;
            bicycle.Station = station;
            db.Update(bicycle);
            if (SaveChanges)
            {
                await db.SaveChangesAsync();
            }
            return new ReturnResult(hire, ReturnResultType.Succeed);
        }

    }
    public class HireResult
    {
        public HireResult()
        {
        }

        public HireResult(Hire hire, HireResultType type)
        {
            Hire = hire;
            Type = type;
        }

        public Hire Hire { get; set; }
        public HireResultType Type { get; set; }
    }
    public class ReturnResult
    {
        public ReturnResult()
        {
        }

        public ReturnResult(Hire hire, ReturnResultType type)
        {
            Hire = hire;
            Type = type;
        }

        public Hire Hire { get; set; }
        public ReturnResultType Type { get; set; }
    }
    public enum HireResultType
    {
        /// <summary>
        /// 成功
        /// </summary>
        Succeed,
        /// <summary>
        /// 用户已经借了一辆车
        /// </summary>
        AnotherIsHired,
        /// <summary>
        /// 内部错误
        /// </summary>
        DatabaseError
    }
    public enum ReturnResultType
    {
        /// <summary>
        /// 成功
        /// </summary>
        Succeed,
        /// <summary>
        /// 找不到记录
        /// </summary>
        RecordNotFound,
        /// <summary>
        /// 租赁点已满
        /// </summary>
        StationIsFull,
        /// <summary>
        /// 租赁点不可还车
        /// </summary>
        StationCannotReturn,
        /// <summary>
        /// 内部错误
        /// </summary>
        DatabaseError
    }
}

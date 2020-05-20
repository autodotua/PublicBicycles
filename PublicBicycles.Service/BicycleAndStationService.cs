using Microsoft.EntityFrameworkCore;
using PublicBicycles.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PublicBicycles.Service
{
    public static class BicycleAndStationService
    {
        /// <summary>
        /// 删除自行车
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bicycleID"></param>
        /// <returns></returns>
        public async static Task<bool> DeleteBicycleAsync(PublicBicyclesContext db, int bicycleID)
        {
            Bicycle bicycle = await db.Bicycles.Where(p => p.ID == bicycleID).Include(p => p.Station).FirstOrDefaultAsync();
            if (bicycle == null)
            {
                return false;
            }
            bicycle.Deleted = true;
            db.Update(bicycle);
            bicycle.Station.BicycleCount--;
            db.Update(bicycle.Station);
            await db.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// 新增自行车
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bicycleID"></param>
        /// <param name="stationID"></param>
        /// <returns></returns>
        public async static Task<bool> AddBicycleAsync(PublicBicyclesContext db, int bicycleID, int stationID)
        {
            var station = await db.Stations.FindAsync(stationID);
            if (station == null || station.BicycleCount >= station.Count)//无法停放更多车辆
            {
                return false;
            }
            Bicycle bicycle = new Bicycle()
            {
                BicycleID = bicycleID,
                Station = station
            };
            db.Bicycles.Add(bicycle);
            station.BicycleCount++;
            db.Update(station);
            await db.SaveChangesAsync();
            return true;
        }
        public async static Task<bool> ModifyBicycleAsync(PublicBicyclesContext db, int id, int bicycleID, bool canHire)
        {
            Bicycle bicycle = await db.Bicycles.FindAsync(id);
            if (bicycle == null)
            {
                return false;
            }
            bicycle.CanHire = canHire;
            bicycle.BicycleID = bicycleID;
            db.Bicycles.Update(bicycle);
            await db.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// 新增租赁点
        /// </summary>
        /// <param name="db"></param>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async static Task AddStationAsync(PublicBicyclesContext db, string name, string address, double lng, double lat, int count)
        {
            Station station = new Station()
            {
                Name = name,
                Address = address,
                Lng = lng,
                Lat = lat,
                Count = count
            };
            db.Stations.Add(station);
            await db.SaveChangesAsync();
        }
        /// <summary>
        /// 修改租赁点信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async static Task<bool> ModifyStationAsync(PublicBicyclesContext db, int id, string name, string address, double lng, double lat, int count)
        {
            Station station = await db.Stations.FindAsync(id);
            if (station == null)
            {
                return false;
            }
            if (count < station.BicycleCount)//租赁点新的容量小于当前车辆数
            {
                return false;
            }
            station.Name = name;
            station.Address = address;
            station.Lng = lng;
            station.Lat = lat;
            station.Count = count;
            db.Stations.Update(station);
            await db.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// 删除租赁点
        /// </summary>
        /// <param name="db"></param>
        /// <param name="stationID"></param>
        /// <returns></returns>
        public async static Task<bool> DeleteStationAsync(PublicBicyclesContext db, int stationID)
        {
            Station station = await db.Stations.FindAsync(stationID);
            if (station == null)
            {
                return false;
            }
            station.Deleted = true;
            db.Update(station);
            await db.SaveChangesAsync();
            return true;
        }
    }
}

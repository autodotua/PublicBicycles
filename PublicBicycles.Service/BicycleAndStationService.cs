using Microsoft.EntityFrameworkCore;
using PublicBicycles.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PublicBicycles.Service
{
    public static class BicycleAndStationService
    {
        public async static Task DeleteBicycleAsync(PublicBicyclesContext db, int bicycleID)
        {
            Bicycle bicycle = await db.Bicycles.Where(p => p.ID == bicycleID).Include(p => p.Station).FirstOrDefaultAsync();
            if (bicycle != null)
            {
                bicycle.Deleted = true;
                db.Update(bicycle);
            }
            bicycle.Station.BicycleCount--;
            db.Update(bicycle.Station);
            await db.SaveChangesAsync();
        }
        public async static Task<bool> AddBicycleAsync(PublicBicyclesContext db, int bicycleID, int stationID)
        {
            var station = db.Stations.Find(stationID);
            if (station == null || station.BicycleCount >= station.Count)
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
        public async static Task<bool> ModifyStationAsync(PublicBicyclesContext db, int id, string name, string address, double lng, double lat, int count)
        {
            Station station = db.Stations.Find(id);
            if (station == null)
            {
                return false;
            }
            if (count > station.BicycleCount)
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
        public async static Task DeleteStationAsync(PublicBicyclesContext db, int stationID)
        {
            Station station = db.Stations.Find(stationID);
            if (station != null)
            {
                station.Deleted = true;
                db.Update(station);
            }
            await db.SaveChangesAsync();
        }
    }
}

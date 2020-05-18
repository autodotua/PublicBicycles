using PublicBicycles.Models;
using System.Threading.Tasks;

namespace PublicBicycles.Service
{
    public static class BicycleAndStationService
    {
        public async static Task DeleteBicycleAsync(PublicBicyclesContext db, int bicycleID)
        {
            Bicycle bicycle = db.Bicycles.Find(bicycleID);
            if (bicycle != null)
            {
                bicycle.Deleted = true;
                db.Update(bicycle);
            }
            await db.SaveChangesAsync();
        }
        public async static Task AddBicycleAsync(PublicBicyclesContext db, int bicycleID, int stationID)
        {
            Bicycle bicycle = new Bicycle()
            {
                BicycleID = bicycleID,
                Station = db.Stations.Find(stationID)
            };
            db.Bicycles.Add(bicycle);
            await db.SaveChangesAsync();
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

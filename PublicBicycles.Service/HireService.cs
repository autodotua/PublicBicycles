using Microsoft.EntityFrameworkCore;
using PublicBicycles.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PublicBicycles.Service
{
    public static class HireService
    {
        internal static Func<DateTime> Now { get; set; } = () => DateTime.Now;
        internal static bool  SaveChanges { get; set; } = true;
        internal static System.Collections.Generic.List<Hire> Hires { get; set; } 
        public static async Task<HireResult> HireAsync(PublicBicyclesContext db, int userID, int bicycleID, int stationID)
        {
            User user = db.Users.Find(userID);
            Station station = db.Stations.Find(stationID);
            Bicycle bicycle = db.Bicycles.Find(bicycleID);
            if (user == null || station == null || bicycle == null)
            {
                return new HireResult(null, HireResultType.DatabaseError);
            }
            Hire hire = await db.Hires.LastOrDefaultRecordAsync(p => p.HireTime.Value, p => p.Hirer.ID == userID && p.ReturnStation == null);
            if (hire != null)
            {
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

            bicycle.Hiring = true;
            bicycle.Station = null;
            db.Update(bicycle);
            if (SaveChanges)
            {
                await db.SaveChangesAsync();
            }
            return new HireResult(hire, HireResultType.Succeed);
        }

        public static async Task<Hire> GetHiringAsync(PublicBicyclesContext db,int userID)
        {
            return await db.Hires
                .OrderByDescending(p => p.HireTime.Value)
                .Where(p => p.Hirer.ID == userID && p.ReturnStation == null)
                .Include(p => p.Bicycle)
                .Include(p => p.HireStation)
                .FirstOrDefaultAsync();
        }
        public static async Task<ReturnResult> ReturnAsync(PublicBicyclesContext db, int userID, int bicycleID, int stationID)
        {
            User user = db.Users.Find(userID);
            Station station = db.Stations.Find(stationID);
            Bicycle bicycle = db.Bicycles.Find(bicycleID);
            Hire hire = await GetHiringAsync(db, userID);
            if(hire==null && Hires!=null && Hires.Any(p=>p.Hirer.ID==userID))
            {
                hire = Hires.First(p => p.Hirer.ID == userID);
            }
            if (user == null || station == null || bicycle == null || hire == null)
            {
                return new ReturnResult(null, ReturnResultType.DatabaseError);
            }
            if (hire == null)
            {
                return new ReturnResult(null, ReturnResultType.RecordNotFound);
            }
            if (!station.CanReturn || !station.Online)
            {
                return new ReturnResult(hire, ReturnResultType.StationCannotReturn);
            }
            if (station.BicycleCount == station.Count)
            {

                return new ReturnResult(hire, ReturnResultType.StationIsFull);
            }
            hire.ReturnStation = station;
            hire.ReturnTime = Now();
            db.Hires.Update(hire);

            station.BicycleCount++;
            db.Stations.Update(station);

            bicycle.Hiring = false;
            bicycle.Station = station;
            db.Update(bicycle);
            if (SaveChanges)
            {
                await db.SaveChangesAsync();
            }
            return new ReturnResult(hire, ReturnResultType.Succeed) ;
        }
    
    }    public class HireResult
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
            Succeed,
            AnotherIsHired,
            DatabaseError
        }
        public enum ReturnResultType
        {
            Succeed,
            RecordNotFound,
            StationIsFull,
            StationCannotReturn,
            DatabaseError
        }
}

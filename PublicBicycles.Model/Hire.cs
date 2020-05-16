using System;
using System.ComponentModel.DataAnnotations;

namespace PublicBicycles.Models
{
    public class Hire : IDbModel
    {

        [Key]
        public int ID { get; set; }
        public DateTime? HireTime { get; set; }
        public Station HireStation { get; set; }
        // public int HireStationID { get; set; }
        public DateTime? ReturnTime { get; set; }
        public Station ReturnStation { get; set; }
        //public int ReturnStationID { get; set; }
        public User Hirer { get; set; }
        public Bicycle Bicycle { get; set; }
        //public int HirerID { get; set; }
    }

}

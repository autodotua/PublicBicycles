using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PublicBicycles.Models
{
    public class Station : IDbModel
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Count { get; set; }
        public int BicycleCount { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        //public List<Bicycle> Bicycles { get; set; }
        public bool CanReturn { get; set; } = true;
        public bool Online { get; set; } = true;
        public bool Deleted { get; set; }

    }

}

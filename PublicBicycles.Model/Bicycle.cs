using System.ComponentModel.DataAnnotations;

namespace PublicBicycles.Models
{
    public class Bicycle : IDbModel
    {
        [Key]
        public int ID { get; set; }
        public Station Station { get; set; }
        //public int StationID { get; set; }
        public bool CanHire { get; set; } = true;
        public bool Deleted { get; set; }
        public bool Hiring { get; set; }
    }

}

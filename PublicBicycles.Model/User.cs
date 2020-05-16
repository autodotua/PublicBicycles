using System.ComponentModel.DataAnnotations;

namespace PublicBicycles.Models
{
    public class User : IDbModel
    {
        [Key]
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string IsAdmin { get; set; }
    }

}

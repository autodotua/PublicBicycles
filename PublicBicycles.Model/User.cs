using System.ComponentModel.DataAnnotations;

namespace PublicBicycles.Models
{
    public class User : IDbModel
    {
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 是否为管理员
        /// </summary>
        public bool IsAdmin { get; set; }
    }

}

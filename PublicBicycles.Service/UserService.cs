using Microsoft.EntityFrameworkCore;
using PublicBicycles.Models;
using System.Text;
using System.Threading.Tasks;

namespace PublicBicycles.Service
{
    /// <summary>
    /// 车主相关
    /// </summary>
    public static class UserService
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="db"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>如果用户名已存在，返回null，否则返回注册后的用户对象</returns>
        public async static Task<LoginOrRegisterResult> RegistAsync(PublicBicyclesContext db, string username, string password)
        {
            if (await db.Users.AnyAsync(p => p.Username == username))
            {
                return new LoginOrRegisterResult() { Type = LoginOrRegisterResultType.Existed };
            }
            User user = new User()
            {
                Username = username,
                Password = CreateMD5(password),
            };
            db.Add(user);
            await db.SaveChangesAsync();
            return new LoginOrRegisterResult() { User = user };
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="db"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async static Task<LoginOrRegisterResult> LoginAsync(PublicBicyclesContext db, string username, string password)
        {
            //判断参数是否为空
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return new LoginOrRegisterResult() { Type = LoginOrRegisterResultType.Empty };
            }
            //寻找用户名与密码都匹配的用户
            User user = await db.Users
                .FirstOrDefaultAsync(p => p.Username == username && p.Password == CreateMD5(password));

            if (user == null)
            {
                //返回用户名或密码错误
                return new LoginOrRegisterResult() { Type = LoginOrRegisterResultType.Wrong };
            }
            db.Entry(user).State = EntityState.Modified;
            //修改并保存用户信息
            await db.SaveChangesAsync();
            return new LoginOrRegisterResult() { User = user };
        }
        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="db"></param>
        /// <param name="user">车主</param>
        /// <param name="password">新密码</param>
        /// <returns></returns>
        public async static Task SetPasswordAsync(PublicBicyclesContext db, User user, string password)
        {
            user.Password = CreateMD5(user.Username + password);
            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
        /// <summary>
        /// 为密码创建MD5。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    } /// <summary>
      /// 登陆注册结果
      /// </summary>
    public class LoginOrRegisterResult
    {
        /// <summary>
        /// 结果类型
        /// </summary>
        public LoginOrRegisterResultType Type { get; set; } = LoginOrRegisterResultType.Succeed;
        public User User { get; set; }
    }
    public enum LoginOrRegisterResultType
    {
        /// <summary>
        /// 成功
        /// </summary>
        Succeed,
        /// <summary>
        /// 用户名或密码为空
        /// </summary>
        Empty,
        /// <summary>
        /// 用户名或密码不正确
        /// </summary>
        Wrong,
        /// <summary>
        /// 用户已存在
        /// </summary>
        Existed,

    }
}

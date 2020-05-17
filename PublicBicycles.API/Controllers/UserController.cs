using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PublicBicycles.Models;
using PublicBicycles.Service;


namespace PublicBicycles.API.Controllers
{
    /// <summary>
    /// 为PublicBicycles.Mobile提供用户相关API
    /// </summary>
    public class UserController : PublicBicyclesControllerBase
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        public async Task<ResponseData<LoginResult>> LoginAsync([FromBody] LoginRequest request)
        {
            var result = await UserService.LoginAsync(db, request.Username, request.Password);
            switch (result.Type)
            {
                case LoginOrRegisterResultType.Succeed:
                    var r = new LoginResult(result.User);
                    HttpContext.Session.SetInt32("user", r.User.ID);
                    return new ResponseData<LoginResult>()
                    {
                        Message = "登陆成功",
                        Data = r
                    };
                case LoginOrRegisterResultType.Empty:
                    return new ResponseData<LoginResult>()
                    {
                        Succeed = false,
                        Message = "用户名或密码为空",
                    };
                case LoginOrRegisterResultType.Wrong:
                    return new ResponseData<LoginResult>()
                    {
                        Succeed = false,
                        Message = "用户名或密码错误",
                    };
                default:
                    throw new NotImplementedException();
            }
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        public async Task<ResponseData<LoginResult>> RegisterAsync([FromBody] LoginRequest request)
        {
            var result = await UserService.RegistAsync(db, request.Username, request.Password);
            switch (result.Type)
            {
                case LoginOrRegisterResultType.Succeed:
                    var r = new LoginResult(result.User);
                    HttpContext.Session.SetInt32("user", r.User.ID);
                    return new ResponseData<LoginResult>()
                    {
                        Message = "注册成功",
                        Data = r
                    };
                case LoginOrRegisterResultType.Existed:
                    return new ResponseData<LoginResult>()
                    {
                        Succeed = false,
                        Message = "用户名已存在",
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        [HttpPost]
        [Route("Records")]
        public async Task<ResponseData<List<Hire>>> HireRecordsAsync([FromBody] UserToken userToken)
        {
            return new ResponseData<List<Hire>>(await db.Hires.
                Where(p => p.Hirer.ID == userToken.UserID)
                .Include(p => p.HireStation)
                .Include(p => p.ReturnStation)
                .ToListAsync());
        }
        [HttpPost]
        [Route("Hire")]
        public async Task<ResponseData<Hire>> HireAsync([FromBody] HireReturnRequest hireRequest)
        {
            var result = await HireService.HireAsync(db, hireRequest.UserID, hireRequest.BicycleID, hireRequest.StationID);
            return result.Type switch
            {
                HireResultType.Succeed => new ResponseData<Hire>(result.Hire),
                HireResultType.AnotherIsHired => new ResponseData<Hire>(result.Hire, false, "借车失败：您已经借了一辆车"),
                HireResultType.DatabaseError => new ResponseData<Hire>(result.Hire, false, "借车失败：内部错误"),
                _ => throw new NotImplementedException(),
            };
        }         
        [HttpPost]
        [Route("Return")]
        public async Task<ResponseData<Hire>> ReturnAsync([FromBody] HireReturnRequest request)
        {
            var result = await HireService.ReturnAsync(db, request.UserID, request.BicycleID, request.StationID);
            return result.Type switch
            {
                ReturnResultType.Succeed => new ResponseData<Hire>(result.Hire),
                ReturnResultType.RecordNotFound => new ResponseData<Hire>(result.Hire, false, "还车失败：没有找到借车记录"),
                ReturnResultType.StationIsFull => new ResponseData<Hire>(result.Hire, false, "还车失败：车站已满"),
                ReturnResultType.StationCannotReturn => new ResponseData<Hire>(result.Hire, false, "还车失败：车站无法还车"),
                ReturnResultType.DatabaseError => new ResponseData<Hire>(result.Hire, false, "还车失败：内部错误"),
                _ => throw new NotImplementedException(),
            };
        }     
        [HttpPost]
        [Route("Status")]
        public async Task<ResponseData<Hire>> StatusAsync([FromBody] UserToken userToken)
        {
            var result = await HireService.GetHiringAsync(db, userToken.UserID);
            return new ResponseData<Hire>(result);
           
        }

        /// <summary>
        /// 登录请求
        /// </summary>
        public class LoginRequest
        {
            /// <summary>
            /// 密码
            /// </summary>
            public string Password { get; set; }
            /// <summary>
            /// 用户名
            /// </summary>
            public string Username { get; set; }
        }
        public class HireReturnRequest : UserToken
        {
            public int BicycleID { get; set; }
            public int StationID { get; set; }
        }

        public class LoginResult : UserToken
        {
            public LoginResult(User user) : base(user.ID, true)
            {
                User = user;
                user.Password = null;
            }

            public User User { get; set; }
        }

    }
}
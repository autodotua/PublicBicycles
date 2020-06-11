﻿using System;
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

        /// <summary>
        /// 获取用户所有借车记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Records")]
        public async Task<ResponseData<List<Hire>>> HireRecordsAsync([FromBody] UserToken request)
        {
            if (!request.IsValid())
            {
                return new ResponseData<List<Hire>>(null, false, "用户验证失败" );
            }
            return new ResponseData<List<Hire>>(await db.Hires.
                Where(p => p.Hirer.ID == request.UserID)
                .Include(p => p.HireStation)
                .Include(p => p.ReturnStation)
                .ToListAsync());
        }
        /// <summary>
        /// 借车
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Hire")]
        public async Task<ResponseData<Hire>> HireAsync([FromBody] HireReturnRequest request)
        {
            if (!request.IsValid())
            {
                return new ResponseData<Hire>(null, false, "用户验证失败");
            }
            var result = await HireService.HireAsync(db, request.UserID, request.BicycleID, request.StationID);
            return result.Type switch
            {
                HireResultType.Succeed => new ResponseData<Hire>(result.Hire),
                HireResultType.AnotherIsHired => new ResponseData<Hire>(result.Hire, false, "借车失败：您已经借了一辆车"),
                HireResultType.DatabaseError => new ResponseData<Hire>(result.Hire, false, "借车失败：内部错误"),
                _ => throw new NotImplementedException(),
            };
        }         
        /// <summary>
        /// 还车
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Return")]
        public async Task<ResponseData<Hire>> ReturnAsync([FromBody] HireReturnRequest request)
        {
            if (!request.IsValid())
            {
                return new ResponseData<Hire>(null, false, "用户验证失败");
            }
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
        /// <summary>
        /// 获取当前是否有借车
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Status")]
        public async Task<ResponseData<Hire>> StatusAsync([FromBody] UserToken request)
        {
            if (!request.IsValid())
            {
                return new ResponseData<Hire>(null, false, "用户验证失败");
            }
            var result = await HireService.GetHiringAsync(db, request.UserID);
            return new ResponseData<Hire>(result);
           
        }

        [HttpPost]
        [Route("Password")]
        public async Task<ResponseData<object>> ChangePasswordAsync([FromBody] ChangePasswordRequest request)
        {
            if (!request.IsValid())
            {
                return new ResponseData<object>() { Succeed = false, Message = "用户验证失败" };
            }
            User carOwner = await db.Users.FindAsync(request.UserID);
            if (carOwner == null)
            {
                return new ResponseData<object>(null, false, "找不到用户");
            }
            if (carOwner.Password != UserService.CreateMD5(carOwner.Username + request.OldPassword))
            {
                return new ResponseData<object>(null, false, "旧密码错误");
            }
            await UserService.SetPasswordAsync(db, carOwner, request.NewPassword);
            return new ResponseData<object>();
        }
        public class ChangePasswordRequest : UserToken
        {
            /// <summary>
            /// 旧密码
            /// </summary>
            public string OldPassword { get; set; }
            /// <summary>
            /// 新密码
            /// </summary>
            public string NewPassword { get; set; }
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
            public LoginResult(User user) : base(user, true)
            {
                User = user;
                user.Password = null;
            }

            public User User { get; set; }
        }

    }
}
﻿using Microsoft.AspNetCore.Mvc;
using PublicBicycles.Service;
using System.Threading.Tasks;

namespace PublicBicycles.API.Controllers
{
    public class AnalysisController : PublicBicyclesControllerBase
    {
        /// <summary>
        /// 获取某一个租赁点的借车还车相互联系的租赁点
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Routes")]
        public async Task<ResponseData<object>> GetRoutesAsync([FromBody] RouteRequest request)
        {
            if (!request.IsValid(true))
            {
                return new ResponseData<object>(null, false, "用户验证失败");
            }
            var result =await StatisticsService.GetStationRoutesAsync(db, request.StationID, request.Days);
            return new ResponseData<object>(result);
        }
    }
    public class RouteRequest : UserToken
    {
        public int StationID { get; set; }
        public int Days { get; set; }
    }
}
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ResponseData> GetRoutesAsync([FromBody] RouteRequest request)
        {
            if (!request.IsValid(true))
            {
                return new ResponseData(null, false, "用户验证失败");
            }
            var result = await StatisticsService.GetStationRoutesAsync(db, request.StationID, request.Days);
            return new ResponseData(result);
        }

        [HttpPost]
        [Route("Leaderboard")]
        public async Task<ResponseData> GetLeaderboardAsync([FromBody] StatisticRequest request)
        {
            if (!request.IsValid(true))
            {
                return new ResponseData(null, false, "用户验证失败");
            }
            var result = await StatisticsService.GetLeaderboardAsync(db, request.Days);
            return new ResponseData(result);
        }   
        [HttpPost]
        [Route("Move")]
        public async Task<ResponseData> GetMoveNeededAsync([FromBody] StatisticRequest request)
        {
            if (!request.IsValid(true))
            {
                return new ResponseData(null, false, "用户验证失败");
            }
            var result = await StatisticsService.GetMoveNeededAsync(db);
            return new ResponseData(result);
        }
    }
    public class RouteRequest : StatisticRequest
    {
        public int StationID { get; set; }
    }
    public class StatisticRequest : UserToken
    {
        public int Days { get; set; }
    }


}
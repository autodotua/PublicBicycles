using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PublicBicycles.Models;
using PublicBicycles.Service;

namespace PublicBicycles.API.Controllers
{
    public class AdminController : PublicBicyclesControllerBase
    {
        [HttpPost]
        [Route("Fake")]
        public ResponseData<object> GenerateTestDatasAsync([FromBody] UserToken request)
        {
            if (!request.IsValid(true))
            {
                return new ResponseData<object>(null, false, "用户验证失败");
            }
            DatabaseInitializer.GenerateTestDatas(db);
            return new ResponseData<object>();
        }
        [HttpPost]
        [Route("Bicycle")]
        public async Task<ResponseData<object>> BicycleAsync([FromBody] BicycleRequest request)
        {
            if (!request.IsValid(true))
            {
                return new ResponseData<object>(null, false, "用户验证失败");
            }

            switch (request.Type)
            {
                case "add":
                    await BicycleAndStationService.AddBicycleAsync(db, request.BicycleID, request.StationID);
                    return new ResponseData<object>();
                case "delete":
                    await BicycleAndStationService.DeleteBicycleAsync(db, request.BicycleID);
                    return new ResponseData<object>();
                default:
                    return new ResponseData<object>(null, false, "不支持的操作类型");
            }
        }
        [HttpPost]
        [Route("Station")]
        public async Task<ResponseData<object>> StationAsync([FromBody] StationRequest request)
        {
            if (!request.IsValid(true))
            {
                return new ResponseData<object>(null, false, "用户验证失败");
            }

            switch (request.Type)
            {
                case "add":

                    return new ResponseData<object>();
                case "delete":
                    await BicycleAndStationService.DeleteStationAsync(db, request.StationID);
                    return new ResponseData<object>();
                default:
                    return new ResponseData<object>(null, false, "不支持的操作类型");
            }
        }
    }

    public class BicycleRequest : UserToken
    {
        public int BicycleID { get; set; }
        public int StationID { get; set; }
        public string Type { get; set; }
    }
    public class StationRequest : UserToken
    {
        public int StationID { get; set; }
        public string Type { get; set; }
    }
}
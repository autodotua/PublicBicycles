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
        public async Task<ResponseData<object>> BicycleAsync([FromBody] CURDRequest<Bicycle> request)
        {
            if (!request.IsValid(true))
            {
                return new ResponseData<object>(null, false, "用户验证失败");
            }

            switch (request.Type)
            {
                case "add":
                    await BicycleAndStationService.AddBicycleAsync(db, request.Item.BicycleID, request.Item.Station.ID);
                    return new ResponseData<object>();
                case "delete":
                    await BicycleAndStationService.DeleteBicycleAsync(db, request.Item.ID);
                    return new ResponseData<object>();
                default:
                    return new ResponseData<object>(null, false, "不支持的操作类型");
            }
        }
        [HttpPost]
        [Route("Station")]
        public async Task<ResponseData<object>> StationAsync([FromBody] CURDRequest<Station> request)
        {
            if (!request.IsValid(true))
            {
                return new ResponseData<object>(null, false, "用户验证失败");
            }

            switch (request.Type)
            {
                case "add":
                    await BicycleAndStationService.AddStationAsync(db,
                        request.Item.Name,
                        request.Item.Address,
                        request.Item.Lng,
                        request.Item.Lat,
                        request.Item.Count);
                    return new ResponseData<object>();
                case "edit":
                    await BicycleAndStationService.ModifyStationAsync(db,
                        request.Item.ID,
                        request.Item.Name,
                        request.Item.Address,
                        request.Item.Lng,
                        request.Item.Lat,
                        request.Item.Count);
                    return new ResponseData<object>();
                case "delete":
                    await BicycleAndStationService.DeleteStationAsync(db, request.Item.ID);
                    return new ResponseData<object>();
                default:
                    return new ResponseData<object>(null, false, "不支持的操作类型");
            }
        }
    }

    public class CURDRequest<T> : UserToken where T : class, new()
    {
        public T Item { get; set; }
        public string Type { get; set; }
    }
}
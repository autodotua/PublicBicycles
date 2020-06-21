using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PublicBicycles.Models;
using PublicBicycles.Service;

namespace PublicBicycles.API.Controllers
{
    public class AdminController : PublicBicyclesControllerBase
    {
        /// <summary>
        /// 生成测试数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Fake")]
        public ResponseData GenerateTestDatasAsync([FromBody] TestDatasRequest request)
        {
            if (!request.IsValid(true))
            {
                return new ResponseData(null, false, "用户验证失败");
            }
            DatabaseInitializer.GenerateTestDatas(db,request.Days);
            return new ResponseData();
        }
        /// <summary>
        /// 对自行车的增删改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Bicycle")]
        public async Task<ResponseData> BicycleAsync([FromBody] CURDRequest<Bicycle> request)
        {
            if (!request.IsValid(true))
            {
                return new ResponseData(null, false, "用户验证失败");
            }
            bool result;
            switch (request.Type)
            {
                case "add":
                    result = await BicycleAndStationService.AddBicycleAsync(db, request.Item.BicycleID, request.Item.Station.ID);
                    return new ResponseData(null, result);
                case "edit":
                    result = await BicycleAndStationService.ModifyBicycleAsync(db, request.Item.ID, request.Item.BicycleID, request.Item.CanHire);
                    return new ResponseData(null, result);
                case "delete":
                    result = await BicycleAndStationService.DeleteBicycleAsync(db, request.Item.ID);
                    return new ResponseData(null, result);
                default:
                    return new ResponseData(null, false, "不支持的操作类型");
            }
        }
        /// <summary>
        /// 对租赁点的增删改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Station")]
        public async Task<ResponseData> StationAsync([FromBody] CURDRequest<Station> request)
        {
            if (!request.IsValid(true))
            {
                return new ResponseData(null, false, "用户验证失败");
            }
            bool result;
            switch (request.Type)
            {
                case "add":
                    await BicycleAndStationService.AddStationAsync(db,
                        request.Item.Name,
                        request.Item.Address,
                        request.Item.Lng,
                        request.Item.Lat,
                        request.Item.Count);
                    return new ResponseData();
                case "edit":
                    result = await BicycleAndStationService.ModifyStationAsync(db,
                           request.Item.ID,
                           request.Item.Name,
                           request.Item.Address,
                           request.Item.Lng,
                           request.Item.Lat,
                           request.Item.Count);
                    return new ResponseData(null, result);
                case "delete":
                    result = await BicycleAndStationService.DeleteStationAsync(db, request.Item.ID);
                    return new ResponseData(null, result);
                default:
                    return new ResponseData(null, false, "不支持的操作类型");
            }
        }
    }
    public class TestDatasRequest : UserToken
    {
        public int Days { get; set; } = 2;
    }
    public class CURDRequest<T> : UserToken where T : class, new()
    {
        /// <summary>
        /// 增删查改的对象
        /// </summary>
        public T Item { get; set; }
        public string Type { get; set; }
    }
}
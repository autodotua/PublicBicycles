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
    }
}
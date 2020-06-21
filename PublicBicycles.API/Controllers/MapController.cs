using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublicBicycles.Models;
using PublicBicycles.Service;

namespace PublicBicycles.API.Controllers
{
    /// <summary>
    /// 为PublicBicycles.Mobile的主页提供相关信息
    /// </summary>
    public class MapController : PublicBicyclesControllerBase
    {
        /// <summary>
        /// 获取所有租赁点的数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Stations")]
        public async Task<ResponseData> StationsAsync()
        {
            var stations = await db.Stations
                .Where(p => p.Online)
                .Where(p=>!p.Deleted)
                .ToListAsync();
            return new ResponseData(stations);
        }
        /// <summary>
        /// 获取某一个租赁点所拥有的所有自行车信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Bicycles/{id}")]
        public async Task<ResponseData> BicyclesAsync(int id)
        {
            var bicycles =await db.Bicycles
                .Where(p => p.Station.ID == id)
                //.Where(p=>p.CanHire)
                .Where(p=>!p.Deleted)
                .ToListAsync();

            return new ResponseData(bicycles);
        }


    }
}

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
        [HttpGet]
        [Route("Stations")]
        public async Task<ResponseData<List<Station>>> StationsAsync()
        {
            var stations = await db.Stations.Where(p => p.Online).ToListAsync();
            return new ResponseData<List<Station>>(stations);
        }
        [HttpGet]
        [Route("Bicycles/{id}")]
        public async Task<ResponseData<List<Bicycle>>> BicyclesAsync(int id)
        {
            var bicycles =await db.Bicycles
                .Where(p => p.Station.ID == id)
                .Where(p=>p.CanHire)
                .Where(p=>!p.Deleted)
                .ToListAsync();

            return new ResponseData<List<Bicycle>>(bicycles);
        }


    }
}

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace PublicBicycles.API.Controllers
{
    /// <summary>
    /// 为Park.Mobile提供停车场相关信息
    /// </summary>
    [ApiController]
    [EnableCors("cors")]
    [Route("[controller]")]
    public abstract class PublicBicyclesControllerBase : ControllerBase
    {
        protected PublicBicyclesControllerBase()
        {
            db = new Context();
        }
        protected Context db { get; private set; }

    }
 
}

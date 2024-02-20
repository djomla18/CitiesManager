using Microsoft.AspNetCore.Mvc;

namespace CitiesManager.WebAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    // Kada citamo API verziju iz rute
    // [Route("api/v{version:apiVersion}/[controller]")]

    [Route("api/[controller]")]
    [ApiController]
    public class CustomControllerBase : ControllerBase
    {
    }
}
using Microsoft.AspNetCore.Mvc;

namespace Evopeons.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public string HealthCheck()
        {
            return "OK";
        }
    }
}
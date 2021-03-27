using Microsoft.AspNetCore.Mvc;

namespace Evopeons.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Get server health status.
        /// </summary>
        /// <returns> Return "OK" if the server runs correctly.</returns>
        [HttpGet]
        public string HealthCheck()
        {
            return "OK";
        }
    }
}
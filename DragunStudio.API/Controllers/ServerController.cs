using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragunStudio.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ServerController : ControllerBase
    {
        /// <summary>
        /// Get server health status.
        /// </summary>
        /// <returns> Return "OK" if the server runs correctly.</returns>
        [HttpGet]
        public ActionResult<string> HealthCheck()
        {
            return Ok("OK");
        }
    }
}

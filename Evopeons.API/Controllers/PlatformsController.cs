using Evorine.EvoPackageManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Evopeons.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public OSType[] GetPlatforms()
        {
            return Enum.GetValues(typeof(OSType)).Cast<OSType>().ToArray();
        }
    }
}
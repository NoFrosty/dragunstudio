using EvoPackageManager.Models;
using Evopeons.API.Articles;
using Evopeons.API.Models;
using Evorine.EvoPackageManager;
using Evorine.EvoPackageManager.DataObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

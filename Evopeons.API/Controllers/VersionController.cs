using Evorine.EvoPackageManager;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evopeons.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private readonly PackageManager packageManager;
        public VersionController(PackageManager packageManager)
        {
            this.packageManager = packageManager;
        }


        [HttpGet("{applicationId}")]
        public async Task<VersionInfo> GetApplicationVersion(string applicationId)
        {
            return await packageManager.GetLastVersion(applicationId);
        }
    }
}

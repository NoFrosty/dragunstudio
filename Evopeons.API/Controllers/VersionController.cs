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

        [HttpGet("last")]
        public async Task<VersionInfo> GetApplicationLastVersion([FromQuery]string applicationId, [FromQuery]int operatingSystem)
        {
            return await packageManager.GetLastVersion(applicationId, (OSType)operatingSystem);
        }

        //[HttpGet("{fromVersionId}/{toVersionId}")]
        //public async Task<IEnumerable<Evorine.EvoPackageManager.DataObjects.UpdateAction>> GetVersionUpdateVersion(string fromVersionId, string toVersionId)
        //{
        //    return await packageManager.GetUpdateAction(fromVersionId, toVersionId);
        //}
    }
}

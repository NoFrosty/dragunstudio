using Evorine.Manager;
using Evorine.Manager.DataObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Evorine.API.Controllers
{
    [Authorize]
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
        public async Task<VersionInfo> GetApplicationLastVersion([FromQuery] Guid applicationId, [FromQuery] int operatingSystem)
        {
            var bidule = await packageManager.GetLastVersion(applicationId, (OSType)operatingSystem);
            return bidule;
        }

        //[HttpGet("{fromVersionId}/{toVersionId}")]
        //public async Task<IEnumerable<Evorine.EvoPackageManager.DataObjects.UpdateAction>> GetVersionUpdateVersion(string fromVersionId, string toVersionId)
        //{
        //    return await packageManager.GetUpdateAction(fromVersionId, toVersionId);
        //}
    }
}
using Evorine.EvoPackageManager;
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
    public class UpdateController
    {
        private readonly PackageManager packageManager;
        public UpdateController(PackageManager packageManager)
        {
            this.packageManager = packageManager;
        }

        [HttpGet]
        public IEnumerable<object> GetUpdateActions([FromQuery]string fromVersionId, [FromQuery]string toVersionId)
        {
            var result = packageManager.GetUpdateAction(fromVersionId, toVersionId).Result;
            return result.Select(x => (object)x);
        }


        [HttpGet("GetInstallationFiles/{operatingSystem}/{applicationId}")]
        public ResponseInstallation GetInstallationFiles(int operatingSystem, string applicationId)
        {
            //TODO: Check operatingSystem
            VersionInfo lastVersion = packageManager.GetLastVersion(applicationId, (OSType)operatingSystem).Result;
            IEnumerable<FileInfo> lastVersionFiles = packageManager.GetFiles(lastVersion.VersionId).Result;
            ResponseInstallation response = new ResponseInstallation
            {
                VersionId = lastVersion.VersionId,
                Version = lastVersion.Version,
                Files = lastVersionFiles
            };
            return response;
        }

        [HttpGet("CheckUpdateAvailable/{operatingSystem}/{applicationId}/{fromVersionId}")]
        public Boolean CheckUpdateAvailable(int operatingSystem, string applicationId, string fromVersionId)
        {
            //TODO: Check operatingSystem
            VersionInfo lastVersion = packageManager.GetLastVersion(applicationId, (OSType)operatingSystem).Result;
            if (packageManager.GetVersion(fromVersionId).Result.VersionOrder < packageManager.GetLastVersion(applicationId, (OSType)operatingSystem).Result.VersionOrder)
                return true;
            else
                return false;
        }



        [HttpGet("GetUpdateActionCopy/{actionId}")]
        public Task<Evorine.EvoPackageManager.DataObjects.UpdateActionCopyFile> GetUpdateActionCopy(string actionId)
        {
            return packageManager.GetUpdateActionCopy(actionId);
        }

        [HttpGet("GetUpdateActionDelete/{actionId}")]
        public Task<Evorine.EvoPackageManager.DataObjects.UpdateActionDeleteFile> GetUpdateActionDelete(string actionId)
        {
            return packageManager.GetUpdateActionDelete(actionId);
        }

        [HttpGet("GetUpdateActionCreate/{actionId}")]
        public Task<Evorine.EvoPackageManager.DataObjects.UpdateActionCreateFile> GetUpdateActionCreate(string actionId)
        {
            return packageManager.GetUpdateActionCreate(actionId);
        }

        [HttpGet("GetUpdateActionUpdate/{actionId}")]
        public Task<Evorine.EvoPackageManager.DataObjects.UpdateActionUpdateFile> GetUpdateActionUpdate(string actionId)
        {
            return packageManager.GetUpdateActionUpdate(actionId);
        }


        public class ResponseInstallation
        {
            public string VersionId { get; set; }
            public string Version { get; set; }
            public IEnumerable<FileInfo> Files { get; set; }
        }
        public class ResponseUpdate
        {
            public string VersionId { get; set; }
            public string Version { get; set; }
            public IEnumerable<Evorine.EvoPackageManager.DataObjects.UpdateAction> UpdateActions { get; set; }
        }
    }
}

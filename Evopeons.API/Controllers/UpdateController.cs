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
    public class UpdateController
    {
        private readonly PackageManager packageManager;
        public UpdateController(PackageManager packageManager)
        {
            this.packageManager = packageManager;
        }


        [HttpGet("GetInstallationFiles/{operatingSystem}/{applicationId}")]
        public ResponseInstallation GetInstallationFiles(int operatingSystem, string applicationId)
        {
            //TODO: Check operatingSystem
            VersionInfo lastVersion = packageManager.GetLastVersion(applicationId).Result;
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
        public Boolean CheckUpdateAvailable(string operatingSystem, string applicationId, string fromVersionId)
        {
            //TODO: Check operatingSystem
            VersionInfo lastVersion = packageManager.GetLastVersion(applicationId).Result;
            if (packageManager.GetVersion(applicationId, fromVersionId).VersionOrder < packageManager.GetLastVersion(applicationId).Result.VersionOrder)
                return true;
            else
                return false;
        }

        [HttpGet("GetUpdateFiles/{operatingSystem}/{applicationId}/{fromVersionId}")]
        public ResponseUpdate GetUpdateFiles(string operatingSystem, string applicationId, string fromVersionId)
        {
            //TODO: Check operatingSystem
            VersionInfo lastVersion = packageManager.GetLastVersion(applicationId).Result;
            IEnumerable<Evorine.EvoPackageManager.DataObjects.UpdateAction> updateActions = packageManager.GetUpdateAction(fromVersionId, lastVersion.VersionId).Result;
            ResponseUpdate response = new ResponseUpdate
            {
                VersionId = lastVersion.VersionId,
                Version = lastVersion.Version,
                UpdateActions = updateActions
            };
            return response;
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

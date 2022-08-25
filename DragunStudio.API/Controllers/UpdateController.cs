using DragunStudio.API.Models.ApplicationViewModels;
using DragunStudio.Managers.Package;
using DragunStudio.Managers.Package.Enums;
using DragunStudio.Managers.Package.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragunStudio.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UpdateController : ControllerBase
    {
        private readonly PackageManager packageManager;

        public UpdateController(PackageManager packageManager)
        {
            this.packageManager = packageManager;
        }

        [HttpGet]
        public IEnumerable<object> GetUpdateActions([FromQuery] Guid? fromVersionId, [FromQuery] Guid toVersionId)
        {
            var result = packageManager.GetUpdateAction(fromVersionId, toVersionId).Result;
            return result.Select(x => (object)x);
        }

        [HttpGet("{versionId}")]
        public async Task<IEnumerable<Update>> GetVersionUpdates(Guid versionId)
        {
            return await packageManager.GetVersionUpdates(versionId);
        }

        [HttpGet("GetInstallationFiles/{operatingSystem}/{applicationId}")]
        public ResponseInstallationResultModel GetInstallationFiles(int operatingSystem, Guid applicationId)
        {
            //TODO: Check operatingSystem
            VersionInfo lastVersion = packageManager.GetLastVersion(applicationId, (OSType)operatingSystem).Result;
            IEnumerable<PackageFileInfo> lastVersionFiles = packageManager.GetFiles(lastVersion.VersionId).Result;
            ResponseInstallationResultModel response = new ResponseInstallationResultModel
            {
                VersionId = lastVersion.VersionId,
                Version = lastVersion.Version,
                Files = lastVersionFiles
            };
            return response;
        }

        [HttpGet("CheckUpdateAvailable/{operatingSystem}/{applicationId}/{fromVersionId}")]
        public Boolean CheckUpdateAvailable(int operatingSystem, Guid applicationId, Guid fromVersionId)
        {
            //TODO: Check operatingSystem
            VersionInfo lastVersion = packageManager.GetLastVersion(applicationId, (OSType)operatingSystem).Result;
            if (packageManager.GetVersion(fromVersionId).Result.VersionOrder < packageManager.GetLastVersion(applicationId, (OSType)operatingSystem).Result.VersionOrder)
                return true;
            else
                return false;
        }

        [HttpGet("GetUpdateActionCopy/{actionId}")]
        public Task<UpdateActionCopyFile> GetUpdateActionCopy(Guid actionId)
        {
            return packageManager.GetUpdateActionCopy(actionId);
        }

        [HttpGet("GetUpdateActionDelete/{actionId}")]
        public Task<UpdateActionDeleteFile> GetUpdateActionDelete(Guid actionId)
        {
            return packageManager.GetUpdateActionDelete(actionId);
        }

        [HttpGet("GetUpdateActionCreate/{actionId}")]
        public Task<UpdateActionCreateFile> GetUpdateActionCreate(Guid actionId)
        {
            return packageManager.GetUpdateActionCreate(actionId);
        }

        [HttpGet("GetUpdateActionUpdate/{actionId}")]
        public Task<UpdateActionUpdateFile> GetUpdateActionUpdate(Guid actionId)
        {
            return packageManager.GetUpdateActionUpdate(actionId);
        }
    }
}

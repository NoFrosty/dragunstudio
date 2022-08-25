using DragunStudio.API.Models.AccountViewModels;
using DragunStudio.API.Models.ApplicationViewModels;
using DragunStudio.Common;
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
    public class ApplicationController : ControllerBase
    {
        private readonly PackageManager packageManager;
        private readonly ApplicationSettings applicationSettings;

        private const long MaxFileSize = 45L * 1024L * 1024L * 1024L;

        public ApplicationController(
            PackageManager packageManager, 
            ApplicationSettings applicationSettings)
        {
            this.packageManager = packageManager;
            this.applicationSettings = applicationSettings;
        }

        /// <summary>
        /// Retrieve a list of all applications.
        /// </summary>
        /// <param name="includeHiddens">Determines if hidden applications will be included.</param>
        /// <param name="extended">If <c>True</c> returns additional properties.</param>
        /// <returns>Returns a collection of <see cref="ApplicationInfo"/> or <see cref="ApplicationViewModel"/> dependings on <paramref name="extended"/>.</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetApplications([FromQuery] bool includeHiddens = false, [FromQuery] bool extended = false)
        {
            if (!extended)
            {
                return Ok(await packageManager.GetApplications(includeHiddens));
            }
            else
            {
                return Ok(await packageManager.GetExtendedApplicationsData(includeHiddens));
            }
        }

        /// <summary>
        /// Retrieve the application associated with specified <paramref name="applicationId"/>.
        /// </summary>
        /// <param name="applicationId">The unique application ID.</param>
        /// <param name="extended">If <c>True</c> returns additional properties.</param>
        /// <returns>Return <see cref="ApplicationInfo"/> or <see cref="ApplicationViewModel"/> dependings on <paramref name="extended"/>.</returns>
        [AllowAnonymous]
        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetApplication(Guid applicationId, [FromQuery] bool extended = false)
        {
            if (!extended)
            {
                return Ok(await packageManager.GetApplication(applicationId));
            }
            else
            {
                return Ok(packageManager.ConvertToApplicationViewModel(await packageManager.GetApplication(applicationId)));
            }
        }

        /// <summary>
        /// Get the application logo associated with specified <paramref name="applicationId"/>.
        /// </summary>
        /// <param name="applicationId">The unique application ID.</param>
        /// <returns> Return the application Logo.</returns>
        [AllowAnonymous]
        [HttpGet("{applicationId}/icon")]
        public IActionResult GetApplicationLogo(Guid applicationId)
        {
            return PhysicalFile(packageManager.GetApplicationLogoPath(applicationId), "image/png");
        }

        /// <summary>
        /// Update the application logo with specified <paramref name="applicationId"/>.
        /// </summary>
        /// <param name="applicationId">The unique application ID.</param>
        /// <param name="icon"></param>
        /// <returns>Returns ok.</returns>
        /// <response code="200">Application logo succesfully updated.</response>
        [AllowAnonymous]
        [HttpPost("{applicationId}/icon")]
        public IActionResult UpdateApplicationLogo(Guid applicationId, IFormFile icon)
        {
            packageManager.UpdateApplicationLogo(applicationId, icon.OpenReadStream());
            return Ok();
        }

        [HttpGet("{applicationId}/versions")]
        public async Task<IEnumerable<VersionInfo>> GetAllVersions(Guid applicationId)
        {
            return await packageManager.GetVersions(applicationId);
        }

        [HttpGet("{applicationId}/version/{versionId}")]
        public async Task<VersionInfo> GetVersion(Guid applicationId, Guid versionId)
        {
            return await packageManager.GetVersion(versionId);
        }

        [HttpGet("{applicationId}/version/{versionId}/updates")]
        public async Task<IEnumerable<Update>> GetUpdates(Guid applicationId, Guid versionId)
        {
            return await packageManager.GetUpdates(versionId);
        }

        [HttpPost]
        public async Task<IActionResult> CreateApplication(ApplicationCreateParameterModel applicationCreateParameterModel)
        {
            if (string.IsNullOrWhiteSpace(applicationCreateParameterModel.Name)) return Problem("An application without a name? Are you serious? Rly???");
            return Ok(packageManager.ConvertToApplicationViewModel(await packageManager.CreateApplication(applicationCreateParameterModel.Name)));
        }

        [HttpPut]
        public IActionResult UpdateApplication(ApplicationInfo model)
        {
            if (string.IsNullOrWhiteSpace(model.ApplicationId.ToString())) return Problem("An application without an ApplicationID can't be updated.");
            packageManager.UpdateApplication(model);
            return Ok();
        }

        [HttpDelete("{applicationId}")]
        public async Task<IActionResult> DeleteApplication(Guid applicationId)
        {
            await packageManager.DeleteApplication(applicationId);
            return Ok();
        }

        [HttpPost("{applicationId}/platform")]
        public async Task<IActionResult> AddPlatformToApplication(Guid applicationId, AddPlatformApplicationParameterModel addPlatformApplicationModel)
        {
            return Ok(packageManager.AddPlatformToApplication(applicationId, addPlatformApplicationModel.OperatingSystem, addPlatformApplicationModel.ExecutablePath));
        }

        [HttpPost("{applicationId}/update")]
        public async Task<IActionResult> AddNewUpdateBetweenVersions(Guid applicationId, AddNewUpdateParameterModel model)
        {
            return Ok(await packageManager.CreateVersion(applicationId, model.FromVersion, model.ToVersion));
        }

        [HttpPost("{applicationId}/version")]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        public async Task<IActionResult> CreateVersion(Guid applicationId, [FromForm] VersionCreateParameterModel versionCreateModel)
        {
            if (versionCreateModel.ZipStream.Length > 0)
            {
                string filePath = Path.Combine(GetTempDirectoryPath(), applicationId.ToString());
                Directory.CreateDirectory(filePath);
                filePath = Path.Combine(filePath, Path.GetTempFileName());

                using (FileStream stream = System.IO.File.Create(filePath))
                {
                    await versionCreateModel.ZipStream.CopyToAsync(stream);
                }
                return Ok(await packageManager.CreateVersion(applicationId, versionCreateModel.NewVersion, versionCreateModel.OperatingSystem, filePath));
            }
            return Problem("Zip stream got a problem");
        }

        [HttpPut("{applicationId}/version/{versionId}")]
        public IActionResult UpdateVersion(VersionInfo version)
        {
            if (string.IsNullOrWhiteSpace(version.VersionId.ToString())) return Problem("A Version without a VersionId can't be updated.");
            packageManager.UpdateVersion(version);
            return Ok();
        }

        [HttpGet("{applicationId}/size")]
        public async Task<IActionResult> GetApplicationSizeInfo(Guid applicationId, [FromQuery] int operatingSystem)
        {
            return Ok(await packageManager.GetApplicationSizeInfo(applicationId, (OSType)operatingSystem));
        }

        [HttpGet("last")]
        public async Task<VersionInfo> GetApplicationLastVersion([FromQuery] Guid applicationId, [FromQuery] int operatingSystem)
        {
            var bidule = await packageManager.GetLastVersion(applicationId, (OSType)operatingSystem);
            return bidule;
        }

        private string GetTempDirectoryPath()
        {
            return Path.Combine(applicationSettings.RootPath, "TempUploadVersion");
        }

        /*
        [AllowAnonymous]
        [HttpGet("{applicationID}/articles")]
        public async Task<IEnumerable<Article>> GetArticles(Guid applicationId, [FromQuery] bool? includeNotPublished = true, [FromQuery] int? firstN = null)
        {
            IEnumerable<Article> articles;
            if (User.IsInRole("Administrators"))
                articles = await articleManager.GetAllArticleByApplication(applicationId, includeNotPublished.Value);
            else
                articles = await articleManager.GetAllArticleByApplication(applicationId, false);
            if (firstN.HasValue)
                return articles.Take(firstN.Value);
            else
                return articles;
        }
        */
    }
}

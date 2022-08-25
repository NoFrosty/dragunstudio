using DragunStudio.Managers.Package;
using DragunStudio.Managers.Package.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DragunStudio.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UpdateFileController : ControllerBase
    {
        private readonly PackageManager packageManager;

        public UpdateFileController(PackageManager packageManager)
        {
            this.packageManager = packageManager;
        }

        [HttpGet("{versionId}")]
        public async Task<IEnumerable<PackageFileInfo>> GetVersionFiles(Guid versionId)
        {
            return await packageManager.GetFiles(versionId);
        }

        [HttpGet("{fileId}")]
        public async Task<PackageFileInfo> GetFile(string fileId)
        {
            return await packageManager.GetFile(fileId);
        }

        [AllowAnonymous]
        [HttpGet("{applicationId}/{fileId}/download")]
        public IActionResult DownloadFile(Guid applicationId, string fileId)
        {
            try
            {
                return File(packageManager.GetFileStream(applicationId, fileId), "application/zip");
            }
            catch (System.IO.FileNotFoundException)
            {
                return NotFound();
            }
        }

        [AllowAnonymous]
        [HttpGet("diff/{applicationId}/{diffId}/download")]
        public IActionResult DownloadDiff(Guid applicationId, string diffId)
        {
            try
            {
                return File(packageManager.GetDiffStream(applicationId, diffId), "application/octet-stream");
            }
            catch (System.IO.FileNotFoundException)
            {
                return NotFound();
            }
        }
    }
}

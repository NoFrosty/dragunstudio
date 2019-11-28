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
    public class FilesController : ControllerBase
    {
        private readonly PackageManager packageManager;
        public FilesController(PackageManager packageManager)
        {
            this.packageManager = packageManager;
        }


        [HttpGet("{versionId}")]
        public async Task<IEnumerable<FileInfo>> GetVersionFiles(string versionId)
        {
            return await packageManager.GetFiles(versionId);
        }

        [HttpGet("{fileId}")]
        public async Task<FileInfo> GetFile(string fileId)
        {
            return await packageManager.GetFile(fileId);
        }

        [HttpGet("{applicationId}/{fileId}/download")]
        public IActionResult DownloadFile(string applicationId, string fileId)
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

        [HttpGet("diff/{applicationId}/{diffId}/download")]
        public IActionResult DownloadDiff(string applicationId, string diffId)
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

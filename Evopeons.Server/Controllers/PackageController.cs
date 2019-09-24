using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evopeons.Common;
using Evopeons.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Evopeons.Server.Controllers
{
    [Route("api/[controller]")]
    public class PackageController : Controller
    {
        public const long MaxFileSize = 5_000_000_000;
        readonly PackageService packageService;
        readonly DataService dataService;

        public PackageController(PackageService packageService, DataService dataService)
        {
            this.packageService = packageService;
            this.dataService = dataService;
        }

        [HttpPost()]
        [RequestSizeLimit(MaxFileSize)]
        public IActionResult CreatePackage(string version, IFormFile file)
        {
            var semVersion = SemVersion.Parse(version);
            packageService.CreateNewPackage(semVersion, file.OpenReadStream());
            return RedirectToPage("/Panel");
        }

        [HttpPost("news")]
        public IActionResult News(string news)
        {
            dataService.SetGenericValue("News", news);
            return RedirectToPage("/Panel");
        }
    }
}

using EvoPackageManager.Models;
using Evopeons.API.Articles;
using Evopeons.API.Models;
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
    public class ApplicationsController : ControllerBase
    {
        private readonly PackageManager packageManager;
        private readonly ArticleManager articleManager;
        public ApplicationsController(PackageManager packageManager, ArticleManager articleManager)
        {
            this.packageManager = packageManager;
            this.articleManager = articleManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetApplications([FromQuery]bool isPublished = true, [FromQuery]bool isVisible = true, [FromQuery]bool extended = false)
        {
            if (!extended)
            {
                return Ok(await packageManager.GetApplications(isPublished));
            }
            else
            {
                return Ok(await packageManager.GetExtendedApplicationsData(isPublished));
            }
        }

        [AllowAnonymous]
        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetApplication(Guid applicationId, [FromQuery]bool extended = false)
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

        [AllowAnonymous]
        [HttpGet("{applicationID}/articles")]
        public async Task<IEnumerable<Article>> GetArticles(Guid applicationID, [FromQuery]bool? includeNotPublished = true, [FromQuery]int? firstN = null)
        {
            IEnumerable<Article> articles;
            if (User.IsInRole("Administrators"))
                articles = await articleManager.GetAllArticleByApplication(applicationID, includeNotPublished.Value);
            else
                articles = await articleManager.GetAllArticleByApplication(applicationID, false);
            if (firstN.HasValue)
                return articles.Take(firstN.Value);
            else
                return articles;
        }

        [HttpPost]
        public async Task<IActionResult> CreateApplication(ApplicationCreateModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name)) return Problem("An application without a name? Are you serious? Rly???");
            return Ok(packageManager.ConvertToApplicationViewModel(await packageManager.CreateApplication(model.Name, OSType.None)));
        }
        /*
        [AllowAnonymous]
        [HttpGet()]
        public async Task<IEnumerable<ApplicationInfo>> GetApplicationsInMaintenance([FromQuery]bool inMaintenance)
        {
            return await packageManager.GetApplications(inMaintenance);
        }
        */


    }
}

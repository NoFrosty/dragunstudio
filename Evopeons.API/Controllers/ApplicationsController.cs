using EvoPackageManager.Models;
using Evopeons.API.Articles;
using Evopeons.API.Models;
using Evorine.EvoPackageManager;
using Evorine.EvoPackageManager.DataObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetApplications([FromQuery]bool includeHiddens = false, [FromQuery]bool extended = false)
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
        [HttpGet("{applicationId}/icon")]
        public IActionResult GetApplicationLogo(Guid applicationId)
        {
            return PhysicalFile(packageManager.GetApplicationLogoPath(applicationId), "image/png");
        }


        [AllowAnonymous]
        [HttpPost("{applicationId}/icon")]
        public IActionResult UpdateApplicationLogo(Guid applicationId, IFormFile icon)
        {
            packageManager.UpdateApplicationLogo(applicationId, icon.OpenReadStream());
            return Ok(); 
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
            return Ok(packageManager.ConvertToApplicationViewModel(await packageManager.CreateApplication(model.Name)));
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

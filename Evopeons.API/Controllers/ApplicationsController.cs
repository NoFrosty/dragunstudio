using Evopeons.API.Articles;
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
        [HttpGet()]
        public async Task<IEnumerable<ApplicationInfo>> GetApplications([FromQuery]bool isPublished = true, [FromQuery]bool isVisible = true)
        {
            return await packageManager.GetApplications(isPublished);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Evopeons.API.Articles;

namespace Evopeons.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        readonly ArticleManager articleManager;
        public ArticleController(ArticleManager articleManager)
        {
            this.articleManager = articleManager;
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<IEnumerable<Article>> GetAllArticle(bool? isPublished, Guid? applicationId, int? firstN)
        {
            IEnumerable<Article> articles;
            if (User.IsInRole("Administrators"))
                articles = await articleManager.GetAllArticles(isPublished ?? false);
            else
                articles = await articleManager.GetAllArticles(false);
            if (firstN.HasValue)
                return articles.Take(firstN.Value);
            else
                return articles;
        }

        [AllowAnonymous]
        [HttpGet("{applicationId}")]
        public async Task<IEnumerable<Article>> GetAllArticleByApplication(Guid applicationId, bool? isPublished, int? firstN)
        {
            IEnumerable<Article> articles;
            if (User.IsInRole("Administrators"))
                articles = await articleManager.GetAllArticleByApplication(applicationId, isPublished ?? false);
            else
                articles = await articleManager.GetAllArticleByApplication(applicationId, false);
            if (firstN.HasValue)
                return articles.Take(firstN.Value);
            else
                return articles;
        }

        [AllowAnonymous]
        [HttpGet("{slug}")]
        public async Task<ActionResult> GetArticle(Guid slug)
        {
            var item = await articleManager.GetArticle(slug);
            if (item == null)
                return NotFound();
            return Ok(item);
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Article article)
        {
            article.Slug = UrlSlugger.ToUrlSlug(article.Title);
            article.PublishDate = DateTime.UtcNow;
            await articleManager.AddArticle(article);
            return Ok();
        }

        [HttpDelete("{slug}")]
        public ActionResult Delete(string slug)
        {
            articleManager.DeleteArticle(slug);
            return Ok();
        }
    }
}

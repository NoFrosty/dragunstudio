using Evorine.API.Articles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Evorine.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ArticleManager articleManager;

        public ArticleController(ArticleManager articleManager)
        {
            this.articleManager = articleManager;
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
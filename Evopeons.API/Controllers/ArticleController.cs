using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Evopeons.API.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Evorine.EvoPackageManager;

namespace Evopeons.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        readonly DataAccess dataAccess;
        public ArticleController(DataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<Article>> GetAllNews()
        {
            return await dataAccess.GetAllArticles(false);
        }

        [AllowAnonymous]
        [HttpGet("{slug}")]
        public async Task<ActionResult> GetNews(string slug)
        {
            var item = await dataAccess.GetArticle(slug);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }


        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Article article)
        {
            article.Slug = UrlSlugger.ToUrlSlug(article.Title);
            article.PublishDate = DateTime.UtcNow;
            await dataAccess.AddArticle(article);
            return Ok();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public string Put(int id, [FromBody] string value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/values/5
        [HttpDelete("{slug}")]
        public void Delete(int slug)
        {
            throw new NotImplementedException();
        }
    }
}

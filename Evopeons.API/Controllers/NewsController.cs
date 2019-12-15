using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Evopeons.API.Models;
using Evopeons.API.Services;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace Evopeons.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        readonly NewsService newsService;
        public NewsController(NewsService newsService)
        {
            this.newsService = newsService;
        }

        [HttpGet]
        public async Task<IEnumerable<News>> GetAllNews()
        {
            //return news.OrderByDescending(x => x.PublishDate);
            return await newsService.GetLastXNews(4);
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult> GetNews(string slug)
        {
            var item = await newsService.GetNews(slug);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }


        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] News news)
        {
            news.Slug = UrlSlugger.ToUrlSlug(news.Title);
            news.PublishDate = DateTime.UtcNow;
            await newsService.AddNews(news);
            return Ok();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public string Put(int id, [FromBody] string value)
        {
            return value;
        }

        // DELETE api/values/5
        [HttpDelete("{slug}")]
        public async Task Delete(int slug)
        {
            await newsService.DeleteNews(slug);
        }
    }
}

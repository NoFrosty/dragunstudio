using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Evopeons_API.Models;

namespace Evopeons_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        News[] news = new News[]
        {
            new News
            {
                Id = 1,
                Slug = "patch-103",
                Title = "Patch 1.0.3 : This is a test !",
                Article = "Cras pharetra eget ipsum vel tincidunt. Phasellus aliquet tortor in lobortis gravida. Sed in pharetra nulla. Ut eu lectus vehicula, maximus odio quis, hendrerit dolor. Fusce condimentum blandit mauris sit amet tempus. Sed accumsan lectus ut enim fringilla feugiat sodales non lectus. Donec tortor elit, ornare a condimentum et, fermentum eget massa. Praesent risus ante, pulvinar at tellus id, tempor tempor mi. Phasellus mi ipsum, hendrerit vitae dictum convallis, tristique a libero.",
                Date = new DateTime(2019,8,01),
                Author = "Frosty",
            },
            new News
            {
                Id = 2,
                Slug = "bug-fix",
                Title = "Bug Fix",
                Article = "Integer cursus ut nisi a tempor. Vivamus rutrum neque in tortor tempus malesuada. Donec a gravida mi. Duis in ligula condimentum, malesuada leo non, auctor dolor. Donec nec bibendum lorem. Phasellus ac sem eleifend, pharetra ante posuere, consectetur augue. Suspendisse eget faucibus enim. Integer felis nisi, imperdiet ac orci a, interdum fermentum ex.",
                Date = new DateTime(2019,8,07),
                Author = "Frosty",
            },
            new News
            {
                Id = 3,
                Slug = "new-combat-system",
                Title = "New Combat System",
                Article = "Vestibulum quis ipsum diam. Sed hendrerit nisl sed ante posuere, non tempus velit fermentum. Praesent gravida at quam in interdum. Nullam molestie mi vitae euismod ullamcorper. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Cras magna nulla, malesuada nec eros sed, fermentum pharetra ex. Nunc mollis viverra pretium. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nulla pharetra augue eu libero placerat scelerisque. Maecenas placerat odio nec venenatis tincidunt. Nunc suscipit ligula magna. Aliquam ornare hendrerit nulla, sed pretium magna suscipit eu. Sed non risus sit amet eros faucibus feugiat quis sit amet dui. Aenean pharetra enim at lacus pellentesque bibendum. In gravida eleifend ex ut gravida. Aenean ultrices odio est, et aliquet massa fermentum eu. Cras id laoreet sem, et cursus orci. Ut sed blandit tellus. Etiam id laoreet urna. Vestibulum vel elementum est. Morbi tincidunt est est, eget dictum felis tristique ut. Nullam vel sollicitudin dui. In aliquam lacus purus, non tristique ligula eleifend faucibus. Donec quam nibh, aliquet vel suscipit elementum, vulputate a risus. Morbi tempus sapien in purus imperdiet, quis dapibus dui finibus. Interdum et malesuada fames ac ante ipsum primis in faucibus. Vestibulum scelerisque dolor vel neque maximus ultrices. Quisque ac erat tempor, molestie nunc quis, auctor diam. Donec finibus turpis dui, eu rutrum libero consequat vitae. Vestibulum fermentum elit in nibh venenatis, ut finibus sem ornare. Sed ligula tortor, bibendum eu velit ut, volutpat posuere mi.",
                Date = new DateTime(2019,8,19),
                Author = "Frosty",
            }
        };
        [HttpGet]
        public IEnumerable<News> GetAllNews()
        {
            return news.OrderByDescending(x => x.Date);
        }
        [HttpGet("{slug}")]
        public ActionResult GetNews(string slug)
        {
            var item = news.FirstOrDefault((p) => p.Slug == slug);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

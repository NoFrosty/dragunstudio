using Dapper.Contrib.Extensions;
using System;

namespace Evopeons.API.Articles
{
    [Table("Article")]
    public class Article
    {
        [ExplicitKey]
        public Guid ArticleId { get; set; }
        public Guid ApplicationID { get; set; }
        //Unique Slug (link of the article)
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }
        public Guid AuthorId { get; set; }
        public Boolean IsPublished { get; set; }
    }
}

using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager
{
    [Table("Article")]
    public class Article
    {
        [ExplicitKey]
        public string Id { get; set; }
        //Unique Slug (link of the article)
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }
        public string AuthorId { get; set; }
        public Boolean Published { get; set; }
        public string ApplicationId { get; set; }
    }
}

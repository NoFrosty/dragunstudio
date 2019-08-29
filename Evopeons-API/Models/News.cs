using Dapper.Contrib.Extensions;
using System;

namespace Evopeons.API.Models
{
    [Table("News")]
    public class News
    {
        //Unique Id
        public int Id { get; set; }
        //Unique Slug (link of the article)
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Article { get; set; }
        public DateTime PublishDate { get; set; }
        //Foreign key to author
        public int Author { get; set; }
        //Default True in database
        public Boolean Published { get; set; }
    }
}

using Dapper.Contrib.Extensions;
using System;

namespace Evopeons.API.Models
{
    [Table("News")]
    public class News
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Article { get; set; }
        public DateTime PublishDate { get; set; }
        public string Author { get; set; }
    }
}

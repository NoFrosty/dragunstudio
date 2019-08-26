using System;

namespace Evopeons.API.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Article { get; set; }
        public DateTime Date { get; set; }
        public string Author { get; set; }
    }
}

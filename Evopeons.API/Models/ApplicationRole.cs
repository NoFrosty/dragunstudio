using Dapper.Contrib.Extensions;

namespace Evopeons.API.Models
{
    [Table("ApplicationRole")]
    public class ApplicationRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
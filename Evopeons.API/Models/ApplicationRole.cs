using Dapper.Contrib.Extensions;

namespace Evorine.API.Models
{
    [Table("ApplicationRole")]
    public class ApplicationRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
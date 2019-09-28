using Dapper.Contrib.Extensions;
using System;

namespace Evopeons.API.Models
{
    [Table("ApplicationRole")]
    public class ApplicationRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

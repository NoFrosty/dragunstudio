using Dapper.Contrib.Extensions;
using System;

namespace Evopeons.API.Models
{
    [Table("UserRoles")]
    public class UserRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

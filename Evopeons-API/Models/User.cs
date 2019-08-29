using Dapper.Contrib.Extensions;
using System;

namespace Evopeons.API.Models
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public AccountStatus Active { get; set; }
        public DateTime SuspensionEnd { get; set; }
        public Role UserRole { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}

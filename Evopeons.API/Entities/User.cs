using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Identity;
using System;

namespace Evorine.API.Entities
{
    [Table("User")]
    //public class User : IdentityUser
    public class User
    {
        [ExplicitKey]
        public Guid UserId { get; set; }

        public string Email { get; set; }
        public string EmailNormalized { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Username { get; set; }
        public string UsernameNormalized { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }

        public DateTime Birthday { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public DateTime SuspensionEnd { get; set; }
        public Role UserRole { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}
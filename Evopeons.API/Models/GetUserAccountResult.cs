using System;

namespace Evopeons.API.Models
{
    public class GetUserAccountResult
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public AccountStatus AccountStatus { get; set; }

        public DateTime LastLoginDate { get; set; }
        public DateTime CreationDate { get; set; }

        public DateTime Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}
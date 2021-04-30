using System;

namespace Evorine.API.Models
{
    public class UpdateUserModel
    {
        public Guid UserID { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Country { get; set; }
    }
}
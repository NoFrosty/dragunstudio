using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evopeons.API.Models
{
    public class CreateUserModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public DateTime Birthday { get; set; }
        public string Country { get; set; }
    }
}

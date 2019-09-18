using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evopeons.API.Models
{
    public class CreateUserResult
    {
        public bool IsSuccessfull { get; set; }
        public bool IsUsernameTaken { get; set; }
        public bool IsEmailTaken { get; set; }
    }
}

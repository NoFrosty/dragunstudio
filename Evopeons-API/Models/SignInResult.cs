using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evopeons.API.Models
{
    public class SignInResult
    {
        public bool IsSuccessfull { get; set; }
        public string Token { get; set; }
    }
}

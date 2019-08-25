using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Evopeons_API.Models;
using System.Security.Cryptography;

namespace Evopeons_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        static string test = Helper.GenerateSalt(8);
        User[] users = new User[]
        {
            new User
            {
                Id = 1,
                UserName = "Frosty",
                FirstName = "Calvin",
                LastName = "Dogus",
                Address1 = "24 Rue Simone Weil",
                Address2 = "",
                PostCode = "31270",
                City = "Cugnaux",
                Birthday = new DateTime(1996,10,03),
                CreationDate = DateTime.Now,
                UserRole = Role.Administrator,
                SuspensionEnd = DateTime.MinValue,
                Active = AccountStatus.Active,
                LastLoginDate = DateTime.MinValue,
                Phone = "0646138090",
                Salt = test,
                Password = Helper.EncodePassword("AzeR6",test)

            },
            new User
            {
                Id = 2,
            },
            new User
            {
                Id = 3,
            }
        };
        [HttpGet]
        public IEnumerable<User> GetAllUsers()
        {
            return users;
        }
        [HttpGet("{pseudo}")]
        public ActionResult GetUser(string userName)
        {
            var item = users.FirstOrDefault((p) => p.UserName == userName);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

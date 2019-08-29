using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Evopeons.API.Models;
using System.Security.Cryptography;

namespace Evopeons.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        static string test = Helper.GenerateSalt(8);

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

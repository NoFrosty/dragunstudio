using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Evopeons.API.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Evopeons.API.Identity;
using Evopeons.API.Entities;

namespace Evopeons.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly UserManager<ApplicationUser> userManager;
        readonly UserStore userStore;

        public UsersController(
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.userStore = userStore;
        }

        [HttpGet]
        public async Task<IQueryable<ApplicationUser>> GetAllUsers()
        {
            return userManager.Users;
        }

        [HttpGet("{pseudo}")]
        public ActionResult GetUser(string userName)
        {
            /*
            var item = users.FirstOrDefault((p) => p.UserName == userName);
            if (item == null)
            {
                return NotFound();
            }
            */
            return Ok(null);
        }

        // POST api/values
        [HttpPost]
        public async void RegisterNewUser([FromBody] CreateUserModel userModel)
        {
            var user = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                Email = userModel.Email,
                Username = userModel.Username,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName
            };

            var result = await userManager.CreateAsync(user, userModel.Password);
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

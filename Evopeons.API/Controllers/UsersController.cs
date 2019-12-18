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
using Microsoft.AspNetCore.Authorization;

namespace Evopeons.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly UserManager<ApplicationUser> userManager;

        public UsersController(
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IQueryable<ApplicationUser>> GetAllUsers()
        {
            return userManager.Users;
        }

        [HttpGet("{username}")]
        public ActionResult<GetUserAccountResult> GetUser(string username)
        {
            
            ApplicationUser user = userManager.FindByNameAsync(username).Result;
            if (user == null)
            {
                return NotFound();
            }
            GetUserAccountResult userResult = new GetUserAccountResult()
            {
                Username = NormalizeString(user.Username),
                Address1 = user.Address1,
                Address2 = user.Address2,
                Birthday = user.Birthday,
                City = user.City,
                Email = NormalizeString(user.Email),
                EmailConfirmed = user.EmailConfirmed,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PostCode = user.PostCode,
                TwoFactorEnabled = user.TwoFactorEnabled
            };


            return Ok(userResult);
        }

        [AllowAnonymous]
        // POST api/values
        [HttpPost]
        public async Task<CreateUserResult> RegisterNewUser([FromBody] CreateUserModel userModel)
        {
            bool isEmailTaken = userManager.FindByEmailAsync(userModel.Email).Result != null;
            bool isUsernameTaken = userManager.FindByNameAsync(userModel.Username).Result != null;
            if(!isEmailTaken && !isUsernameTaken)
            {
                var user = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = userModel.Email,
                    EmailNormalized = userModel.Email,
                    Username = userModel.Username,
                    UsernameNormalized = userModel.Username,
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    Birthday = userModel.Birthday,
                    Country = userModel.Country,
                    Active = AccountStatus.Active,
                    CreationDate = DateTime.UtcNow,
                    TwoFactorEnabled = false,
                };

                var result = await userManager.CreateAsync(user, userModel.Password);

                return new CreateUserResult()
                {
                    IsSuccessfull = result.Succeeded,
                    IsEmailTaken = isEmailTaken,
                    IsUsernameTaken = isUsernameTaken
                };
            }
            return new CreateUserResult()
            {
                IsSuccessfull = false,
                IsEmailTaken = isEmailTaken,
                IsUsernameTaken = isUsernameTaken
            };

        }

        public string NormalizeString(string value)
        {
            return char.ToUpper(value[0]) + value.Substring(1).ToLower();
        }
    }
}

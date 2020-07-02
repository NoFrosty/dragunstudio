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
using RestSharp;
using RestSharp.Authenticators;
using System.IO;
using System.Reflection;

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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<GetUserAccountResult>> GetAllUsers()
        {
            List<GetUserAccountResult> users = new List<GetUserAccountResult>();
            foreach (ApplicationUser user in userManager.Users)
            {
                GetUserAccountResult u = new GetUserAccountResult()
                {
                    Username = NormalizeString(user.Username),
                    Email = NormalizeString(user.Email),
                    EmailConfirmed = user.EmailConfirmed,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    CreationDate = user.CreationDate,
                    LastLoginDate = user.LastLoginDate,
                    UserId = user.UserId,
                    AccountStatus = user.AccountStatus
                };
                users.Add(u);
            }
            return users;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<GetUserAccountResult>> GetUser(Guid userId)
        {
            
            ApplicationUser user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }
            GetUserAccountResult userResult = new GetUserAccountResult()
            {
                Username = NormalizeString(user.Username),
                Email = NormalizeString(user.Email),
                EmailConfirmed = user.EmailConfirmed,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                TwoFactorEnabled = user.TwoFactorEnabled,
                CreationDate = user.CreationDate,
                LastLoginDate = user.LastLoginDate,
                UserId = user.UserId,
                Birthday = user.Birthday,
                AccountStatus = user.AccountStatus,
            };


            return Ok(userResult);
        }

        [AllowAnonymous]
        // POST api/values
        [HttpPost]
        public async Task<CreateUserResult> RegisterNewUser([FromBody] CreateUserModel userModel)
        {
            ApplicationUser userByEmail = await userManager.FindByEmailAsync(userModel.Email);
            ApplicationUser userByUserName = await userManager.FindByNameAsync(userModel.Username);

            if ((userByEmail != null && userByEmail.AccountStatus != AccountStatus.Deleted) || userByUserName != null)
                return new CreateUserResult()
                {
                    IsSuccessfull = false,
                    IsEmailTaken = userByEmail != null,
                    IsUsernameTaken = userByUserName != null
                };

            ApplicationUser user = new ApplicationUser()
            {
                UserId = Guid.NewGuid(),
                Email = userModel.Email,
                EmailNormalized = userModel.Email,
                Username = userModel.Username,
                UsernameNormalized = userModel.Username,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Birthday = userModel.Birthday,
                AccountStatus = AccountStatus.Active,
                CreationDate = DateTime.UtcNow,
                TwoFactorEnabled = false,
            };
            var x = SendConfirmationEmail(user);

            IdentityResult result;
            if (userByEmail != null)
            {
                user.UserId = userByEmail.UserId;
                user.PasswordHash = userManager.PasswordHasher.HashPassword(user, userModel.Password);
                result = await userManager.UpdateAsync(user);
            }
            else
            {
                result = await userManager.CreateAsync(user, userModel.Password);
            }
            return new CreateUserResult()
            {
                IsSuccessfull = result.Succeeded,
                IsEmailTaken = userByEmail != null,
                IsUsernameTaken = userByUserName != null
            };

        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            if (string.IsNullOrWhiteSpace(userId.ToString())) return Problem("An userId is necessary !");

            ApplicationUser user = await userManager.FindByIdAsync(userId.ToString());

            user.AccountStatus = AccountStatus.Deleted;
            user.Username = Guid.NewGuid().ToString().Substring(0,20);
            user.UsernameNormalized = userManager.NormalizeName(user.Username);
            return Ok(userManager.UpdateAsync(user));

        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(ApplicationUser userModel)
        {
            if (string.IsNullOrWhiteSpace(userModel.UserId.ToString())) return Problem("A user without an UserId can't be updated.");
            ApplicationUser user = await userManager.FindByIdAsync(userModel.UserId.ToString());
            userModel.PasswordHash = user.PasswordHash;
            userModel.UsernameNormalized = userManager.NormalizeName(userModel.Username);
            userModel.EmailNormalized = userManager.NormalizeEmail(userModel.Email);
            return Ok(await userManager.UpdateAsync(userModel));
        }

        public string NormalizeString(string value)
        {
            return char.ToUpper(value[0]) + value.Substring(1).ToLower();
        }

        [AllowAnonymous]
        [HttpGet("confirmation")]
        public async Task<ActionResult> ConfirmEmail(Guid userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }
            var user = await userManager.FindByIdAsync(userId.ToString());
            var result = await userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Ok("Email Confirmed");
            }
            return Ok();
        }

        public async Task<IRestResponse> SendConfirmationEmail(ApplicationUser user)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");

            client.Authenticator = new HttpBasicAuthenticator("api", "ff7082d5dc1f5b087b9e2de4e32eb323-913a5827-1ed37e1f");

            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandbox7114b6aaa893439b81282df32247dd80.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Evorine <mailgun@sandbox7114b6aaa893439b81282df32247dd80.mailgun.org>");
            request.AddParameter("to", user.Email);
            request.AddParameter("subject", "Account confirmation email");
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Pages\ConfirmEmail.html");
            string htmlText = System.IO.File.ReadAllText("Pages/ConfirmEmail.html");
            string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action(
               "ConfirmEmail", "users",
               new { userId = user.UserId, code = code },
               protocol: Request.Scheme,
               Request.Host.Value);
            htmlText = htmlText.Replace("{url}", callbackUrl);
            htmlText = htmlText.Replace("{firstName}", user.FirstName);
            request.AddParameter("html", htmlText);
            request.Method = Method.POST;

            return client.Execute(request);
        }
    }
}

using Evopeons.API.Entities;
using Evopeons.API.Models;
using Evopeons.API.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Evopeons.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationSettings applicationSettings;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            ApplicationSettings applicationSettings)
        {
            this.userManager = userManager;
            this.applicationSettings = applicationSettings;
        }

        /// <summary>
        /// Gets all Users.
        /// </summary>
        /// <returns>Returns a collection of <see cref="GetUserAccountResult"/>.</returns>
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
                    AccountStatus = user.AccountStatus,
                };
                users.Add(u);
            }
            return users;
        }

        /// <summary>
        /// Get the user associated with specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The unique user ID.</param>
        /// <returns>Return <see cref="GetUserAccountResult"/>.</returns>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(GetUserAccountResult), 200)]
        [ProducesResponseType(404)]
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

        /// <summary>
        /// Create the specified user.
        /// </summary>
        /// <param name="userModel">Specified user to create.</param>
        /// <returns>Return a <see cref="CreateUserResult"/> that represent the status of the operation.</returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(CreateUserResult), 200)]
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
                Country = userModel.Country
            };

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

            await SendConfirmationEmailAmazon(user);

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
            user.Username = Guid.NewGuid().ToString().Substring(0, 20);
            user.UsernameNormalized = userManager.NormalizeName(user.Username);
            return Ok(userManager.UpdateAsync(user));
        }

        /// <summary>
        /// Update user associated with specified <paramref name="userModel"/>.UserId.
        /// </summary>
        /// <param name="userModel">New user data</param>
        /// <returns>Return the new user represented by <see cref="IdentityResult"/></returns>
        [HttpPut]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        public async Task<IActionResult> UpdateUser(UpdateUserModel userModel)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userModel.Email);
            if (user == null) return Problem("User doesn't exist");
            user.Username = userModel.Username;
            user.Email = userModel.Email;
            user.Username = userModel.Username;
            user.FirstName = userModel.FirstName;
            user.LastName = userModel.LastName;
            user.Birthday = userModel.Birthday;
            user.Country = userModel.Country;
            user.UsernameNormalized = userManager.NormalizeName(userModel.Username);
            user.EmailNormalized = userManager.NormalizeEmail(userModel.Email);
            return Ok(await userManager.UpdateAsync(user));
        }

        /// <summary>
        /// Update user password associated with specified <paramref name="updateUserPasswordModel"/>.UserId.
        /// </summary>
        /// <param name="updateUserPasswordModel">New user data</param>
        /// <returns>Return the new user represented by <see cref="IdentityResult"/></returns>
        [HttpPut("password")]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        public async Task<IActionResult> UpdateUserPassword(UpdateUserPasswordModel updateUserPasswordModel)
        {
            ApplicationUser user = await userManager.FindByIdAsync(updateUserPasswordModel.UserID.ToString());
            if (user == null) return Problem("User doesn't exist");
            return Ok(await userManager.ChangePasswordAsync(user, updateUserPasswordModel.CurrentPassword, updateUserPasswordModel.NewPassword));
        }

        private string NormalizeString(string value)
        {
            return char.ToUpper(value[0]) + value.Substring(1).ToLower();
        }

        [AllowAnonymous]
        [HttpGet("confirmation")]
        public async Task<ActionResult> ConfirmEmail(Guid userId, string code)
        {
            if (userId == Guid.Empty || code == null)
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

        [AllowAnonymous]
        [HttpGet("count")]
        public async Task<ActionResult> CountUser()
        {
            var x = userManager.Users.Count();
            return Ok(x);
        }

        private async Task SendConfirmationEmailAmazon(ApplicationUser user)
        {
            string BODY = Resources.ConfirmEmail;
            string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action(
               "ConfirmEmail", "users",
               new { userId = user.UserId, code = code },
               protocol: Request.Scheme
               //Request.Host.Value
               );
            BODY = BODY.Replace("{url}", callbackUrl);
            BODY = BODY.Replace("{firstName}", user.FirstName);

            // Create and build a new MailMessage object
            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(applicationSettings.AmazonEmailSender, "Evorine");
            message.To.Add(new MailAddress(user.Email));
            message.Subject = "Evorine Email Account Verification";
            message.Body = BODY;
            // Comment or delete the next line if you are not using a configuration set
            //message.Headers.Add("X-SES-CONFIGURATION-SET", CONFIGSET);

            using (var client = new SmtpClient(applicationSettings.AmazonHost, 587))
            {
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential(applicationSettings.AmazonSmtpUsername, applicationSettings.AmazonSmtpPassword);

                // Enable SSL encryption
                client.EnableSsl = true;

                // Try to send the message. Show status in console.
                try
                {
                    Console.WriteLine("Attempting to send email...");
                    client.Send(message);
                    Console.WriteLine("Email sent!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);
                }
            }
        }
    }
}
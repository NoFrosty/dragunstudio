using Evopeons.API.Entities;
using Evopeons.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Evopeons.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationSettings applicationSettings;

        public AuthenticateController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ApplicationSettings applicationSettings)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.applicationSettings = applicationSettings;
        }

        [HttpGet]
        public object Status()
        {
            return new
            {
                IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
                Identity = HttpContext.User.Identity.Name
            };
        }

        /// <summary>
        /// Sign in the user with specified user <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The user email and password model.</param>
        /// <returns>Return if sign in was succesfull, if it's succesfull, return also token and username.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Models.SignInResult), 200)]
        [ProducesResponseType(typeof(Models.SignInResult), 403)]
        public async Task<ObjectResult> SignIn(SignInModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return StatusCode(403, new Models.SignInResult()
                {
                    IsSuccessfull = false
                });
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
            {
                return StatusCode(403, new Models.SignInResult()
                {
                    IsSuccessfull = false
                });
            }

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
            };
            var claimsIdentity = new ClaimsIdentity(claims);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(applicationSettings.ApplicationSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            user.LastLoginDate = DateTime.UtcNow;
            await userManager.UpdateAsync(user);

            return StatusCode(200, new Models.SignInResult()
            {
                IsSuccessfull = true,
                Token = tokenHandler.WriteToken(token),
                Username = user.Username
            });
        }

        /// <summary>
        /// Signs the current user out of the API.
        /// </summary>
        [HttpPost]
        [Route("signout")]
        public async void SignOut()
        {
            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
        }
    }
}
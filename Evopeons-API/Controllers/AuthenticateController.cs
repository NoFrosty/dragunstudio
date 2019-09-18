using Evopeons.API.Entities;
using Evopeons.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Evopeons.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        readonly SignInManager<ApplicationUser> signInManager;
        readonly UserManager<ApplicationUser> userManager;
        readonly ApplicationSettings applicationSettings;

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

        [HttpPost]
        public async Task<Models.SignInResult> SignIn(SignInModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
            {
                return new Models.SignInResult()
                {
                    IsSuccessfull = false
                };
            }
            
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
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

            return new Models.SignInResult()
            {
                IsSuccessfull = true,
                Token = tokenHandler.WriteToken(token)
            };
        }

        [HttpPost]
        [Route("signout")]
        public async Task SignOut()
        {
            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
        }
    }
}

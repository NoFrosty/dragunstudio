using Evorine.API.Entities;
using Evorine.API.JwtFeatures;
using Evorine.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Evorine.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly ApplicationSettings applicationSettings;
        private readonly JwtHandler jwtHandler;

        public AuthenticateController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ApplicationSettings applicationSettings,
            JwtHandler jwtHandler)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.applicationSettings = applicationSettings;
            this.jwtHandler = jwtHandler;
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
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                return StatusCode(403, new Models.SignInResult()
                {
                    IsSuccessfull = false
                });
            }
            /*
            if (result.RequiresTwoFactor)
            {
                RedirectToAction("LoginTwoStep", new { user.Email, model });
            }
            */
            SigningCredentials signingCredentials = jwtHandler.GetSigningCredentials();
            List<Claim> claims = jwtHandler.GetClaims(user);
            JwtSecurityToken tokenOptions = jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            string token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            user.LastLoginDate = DateTime.UtcNow;
            await userManager.UpdateAsync(user);

            return StatusCode(200, new Models.SignInResult()
            {
                IsSuccessfull = true,
                Token = token,
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
using DragunStudio.API.Enums;
using DragunStudio.API.Models;
using DragunStudio.API.Models.AccountViewModels;
using DragunStudio.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DragunStudio.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthentificationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly JWTHandler jwtHandler;

        public AuthentificationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            JWTHandler jwtHandler)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtHandler = jwtHandler;
        }

        /// <summary>
        /// Sign in the user with specified user <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The user email and password model.</param>
        /// <returns>Return if sign in was succesfull, if it's succesfull, return also token and username.</returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(SignInResultModel), 200)]
        [ProducesResponseType(typeof(SignInResultModel), 403)]
        public async Task<ObjectResult> SignIn(SignInParameterModel signInParameterModel)
        {
            var user = await userManager.FindByEmailAsync(signInParameterModel.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, signInParameterModel.Password))
            {
                return StatusCode(403, new SignInResultModel()
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

            user.LastLoginDateTime = DateTimeOffset.Now;
            await userManager.UpdateAsync(user);

            return StatusCode(200, new SignInResultModel()
            {
                IsSuccessfull = true,
                Token = token,
                UserName = user.UserName
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

using DragunStudio.API.Enums;
using DragunStudio.API.Models;
using DragunStudio.API.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DragunStudio.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AccountController(
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// Retrieve a list of all users.
        /// </summary>
        /// <returns>Returns a collection of <see cref="GetUserResultModel"/>.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetUserResultModel>), 200)]
        [ProducesResponseType(typeof(IEnumerable<GetUserResultModel>), 404)]
        public async Task<ActionResult<IEnumerable<GetUserResultModel>>> GetAllUsers()
        {
            List<GetUserResultModel> users = new List<GetUserResultModel>();
            foreach (ApplicationUser user in userManager.Users)
            {
                GetUserResultModel u = new GetUserResultModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    AccessFailedCount = user.AccessFailedCount,
                    AccountStatus = user.AccountStatus,
                    Address1 = user.Address1,
                    Address2 = user.Address2,
                    Birthday = user.Birthday,
                    Country = user.Country,
                    CreationDateTime = user.CreationDateTime,
                    LastLoginDateTime = user.LastLoginDateTime,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    NormalizedEmail = user.NormalizedEmail,
                    NormalizedUserName = user.NormalizedUserName,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    PostCode = user.PostCode,
                    TwoFactorEnabled = user.TwoFactorEnabled,

                };
                users.Add(u);
            }
            if (users.Count > 0)
                return Ok(users);
            return NotFound(users);
        }

        /// <summary>
        /// Retrieve the user associated with specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The unique user ID.</param>
        /// <returns>Return <see cref="GetUserAccountResult"/>.</returns>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(GetUserResultModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<GetUserResultModel>> GetUser(string userId)
        {
            if(userId == null || userId == "")
                return BadRequest();
            else
            {
                ApplicationUser user = await userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound();
                else
                {
                    GetUserResultModel userResult = new GetUserResultModel()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        AccessFailedCount = user.AccessFailedCount,
                        AccountStatus = user.AccountStatus,
                        Address1 = user.Address1,
                        Address2 = user.Address2,
                        Birthday = user.Birthday,
                        Country = user.Country,
                        CreationDateTime = user.CreationDateTime,
                        LastLoginDateTime = user.LastLoginDateTime,
                        LockoutEnabled = user.LockoutEnabled,
                        LockoutEnd = user.LockoutEnd,
                        NormalizedEmail = user.NormalizedEmail,
                        NormalizedUserName = user.NormalizedUserName,
                        PhoneNumber = user.PhoneNumber,
                        PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                        PostCode = user.PostCode,
                        TwoFactorEnabled = user.TwoFactorEnabled
                    };
                    return Ok(userResult);
                }
            }
        }

        /// <summary>
        /// Create a new user with specified data <paramref name="userModel"/>.
        /// </summary>
        /// <param name="userModel">Specified user to create.</param>
        /// <returns>Return a <see cref="CreateUserResult"/> that represent the status of the operation.</returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(RegisterResultModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(RegisterResultModel), 400)]
        public async Task<ActionResult<RegisterResultModel>> RegisterUser([FromBody] RegisterParameterModel registerParameterModel)
        {
            if (registerParameterModel == null)
                return BadRequest();
            else
            {
                ApplicationUser userByEmail = await userManager.FindByEmailAsync(registerParameterModel.Email);
                ApplicationUser userByUserName = await userManager.FindByNameAsync(registerParameterModel.UserName);

                if ((userByEmail != null && userByEmail.AccountStatus != AccountStatus.Deleted) || userByUserName != null)
                    return BadRequest(new RegisterResultModel()
                    {
                        IsSuccessfull = false,
                        IsEmailTaken = userByEmail != null,
                        IsUsernameTaken = userByUserName != null
                    });

                ApplicationUser user = new ApplicationUser()
                {
                    Email = registerParameterModel.Email,
                    UserName = registerParameterModel.UserName,
                    FirstName = registerParameterModel.FirstName,
                    LastName = registerParameterModel.LastName,
                    Birthday = registerParameterModel.Birthday,
                    AccountStatus = AccountStatus.Active,
                    CreationDateTime = DateTimeOffset.UtcNow,
                    Country = registerParameterModel.Country
                };

                IdentityResult result;
                if (userByEmail != null && userByEmail.AccountStatus == AccountStatus.Deleted)
                {
                    userByEmail.AccountStatus = AccountStatus.Active;
                    result = await userManager.UpdateAsync(userByEmail);
                    if (result.Succeeded)
                    {
                        await userManager.RemovePasswordAsync(userByEmail);
                        await userManager.AddPasswordAsync(userByEmail, registerParameterModel.Password);
                    }
                }
                else
                {
                    result = await userManager.CreateAsync(user, registerParameterModel.Password);
                }

                return Ok(new RegisterResultModel()
                {
                    IsSuccessfull = result.Succeeded,
                    IsEmailTaken = userByEmail != null,
                    IsUsernameTaken = userByUserName != null,
                    IdentityResult = result
                });
            }
        }

        /// <summary>
        /// Delete the user associated to the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Specified userId to delete.</param>
        /// <returns>Return a <see cref="CreateUserResult"/> that represent the status of the operation.</returns>
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(IdentityResult), 400)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) 
                return BadRequest("An userId is necessary !");

            ApplicationUser user = await userManager.FindByIdAsync(userId);

            user.AccountStatus = AccountStatus.Deleted;
            user.PhoneNumber = null;
            user.UserName = null;
            user.FirstName = null;
            user.LastName = null;
            user.Email = null;
            user.EmailConfirmed = false;
            user.AccessFailedCount = -1;
            user.Address1 = null;
            user.Address2 = null;
            user.Birthday = null;
            user.Country = null;
            user.CreationDateTime = null;
            user.LastLoginDateTime = DateTimeOffset.Now;
            user.LockoutEnabled = false;
            user.LockoutEnd = null;
            user.NormalizedEmail = null;
            user.NormalizedUserName = null;
            user.PhoneNumberConfirmed = false;
            user.PostCode = null;
            user.TwoFactorEnabled = false;
            
            IdentityResult result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
                result = await userManager.RemovePasswordAsync(user);
            if (result.Succeeded)
                return BadRequest(result);
            else
                return Ok(result);
        }

        /// <summary>
        /// Update user associated with specified <paramref name="updateUserParameterModel"/>.UserId.
        /// </summary>
        /// <param name="updateUserParameterModel">New user data</param>
        /// <returns>Return the status of the opération represented by <see cref="IdentityResult"/></returns>
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> UpdateUser(string userId, UpdateUserParameterModel updateUserParameterModel)
        {
            ApplicationUser user = await userManager.FindByIdAsync(updateUserParameterModel.Id);

            if (user == null) 
                return BadRequest("User doesn't exist");

            user.UserName = updateUserParameterModel.UserName;
            user.Email = updateUserParameterModel.Email;
            user.FirstName = updateUserParameterModel.FirstName;
            user.LastName = updateUserParameterModel.LastName;
            user.Birthday = updateUserParameterModel.Birthday;
            user.Country = updateUserParameterModel.Country;
            IdentityResult result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await userManager.UpdateNormalizedEmailAsync(user);
                await userManager.UpdateNormalizedUserNameAsync(user);
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Update user password associated with specified <paramref name="updatePasswordParameterModel"/>.UserId.
        /// </summary>
        /// <param name="updatePasswordParameterModel">New user data</param>
        /// <returns>Return the new user represented by <see cref="IdentityResult"/></returns>
        [HttpPut("password")]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        public async Task<IActionResult> UpdateUserPassword(UpdatePasswordParameterModel updatePasswordParameterModel)
        {
            ApplicationUser user = await userManager.FindByIdAsync(updatePasswordParameterModel.Id);

            if (user == null) 
                return Problem("User doesn't exist");
            else
                return Ok(await userManager.ChangePasswordAsync(user, updatePasswordParameterModel.CurrentPassword, updatePasswordParameterModel.NewPassword));
        }

        /// <summary>
        /// Retrieve the number of User registered.
        /// </summary>
        /// <returns>Return the of user.</returns>
        [HttpGet("count")]
        public async Task<ActionResult> CountUser()
        {
            return Ok(userManager.Users.Count());
        }
    }
}

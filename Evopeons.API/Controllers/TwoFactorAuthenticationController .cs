using Evorine.API.Entities;
using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evorine.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TwoFactorAuthenticationController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        public TwoFactorAuthenticationController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }
            /*

        [HttpGet]
        public async Task<ActionResult> Enable(Guid userId)
        {
            if (string.IsNullOrWhiteSpace(userId.ToString())) return Problem("An userId is necessary !");
            ApplicationUser user = await userManager.FindByIdAsync(userId.ToString());
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            UserDocument;
            var setupInfo =
                twoFactor.GenerateSetupCode("myapp", user.Email, TwoFactorKey(user), false, 3);
            ViewBag.SetupCode = setupInfo.ManualEntryKey;
            ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            return View();
        }
        private static string TwoFactorKey(UserDocument user)
        {
            return $"myverysecretkey+{user.Email}";
        }
            */
    }
}

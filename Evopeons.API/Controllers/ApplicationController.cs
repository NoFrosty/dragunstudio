using Evorine.EvoPackageManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evopeons.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly PackageManager packageManager;
        public ApplicationController(PackageManager packageManager)
        {
            this.packageManager = packageManager;
        }
        
        [AllowAnonymous]
        [HttpGet("{includeHidden}")]
        public async Task<IEnumerable<ApplicationInfo>> GetApplications(bool includeHidden)
        {
            return await packageManager.GetApplications(includeHidden);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<ApplicationInfo>> GetApplications()
        {
            return await packageManager.GetApplications(false);
        }


    }
}

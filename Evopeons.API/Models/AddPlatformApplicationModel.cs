using Evorine.EvoPackageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evopeons.API.Models
{
    public class AddPlatformApplicationModel
    {
        public OSType OperatingSystem { get; set; }
        public string ExecutablePath { get; set; }
    }
}

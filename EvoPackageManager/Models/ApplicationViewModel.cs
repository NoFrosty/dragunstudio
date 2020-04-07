using Evorine.EvoPackageManager.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoPackageManager.Models
{
    public class ApplicationViewModel : ApplicationInfo
    {
        public VersionInfo[] LastVersions { get; set; }
        public PlatformInfo[] Platforms { get; set; }
    }
}

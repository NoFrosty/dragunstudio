using Evorine.EvoPackageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoPackageManager.Models
{
    public class ApplicationViewModel : ApplicationInfo
    {
        public VersionInfo[] LastVersions { get; set; }
        public OSType[] SupportedOSs => Enum
            .GetValues(typeof(OSType))
            .Cast<int>()
            .Where(x => (x & (int)SupportedOS) == x && x != 0)
            .Cast<OSType>()
            .ToArray();
    }
}

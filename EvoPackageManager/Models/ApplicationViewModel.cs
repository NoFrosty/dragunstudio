using Evorine.EvoPackageManager.DataObjects;

namespace EvoPackageManager.Models
{
    public class ApplicationViewModel : ApplicationInfo
    {
        public VersionInfo[] LastVersions { get; set; }
        public PlatformInfo[] Platforms { get; set; }
    }
}
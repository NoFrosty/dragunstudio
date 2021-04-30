using Evorine.Manager.DataObjects;

namespace Evorine.Manager.Models
{
    public class ApplicationViewModel : ApplicationInfo
    {
        public VersionInfo[] LastVersions { get; set; }
        public PlatformInfo[] Platforms { get; set; }
    }
}
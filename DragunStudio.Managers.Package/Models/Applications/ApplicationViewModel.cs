namespace DragunStudio.Managers.Package.Models.Applications
{
    public class ApplicationViewModel : ApplicationInfo
    {
        public VersionInfo[] LastVersions { get; set; }
        public PlatformInfo[] Platforms { get; set; }
    }
}
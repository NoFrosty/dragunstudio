using Evorine.EvoPackageManager;

namespace Evopeons.API.Models
{
    public class AddPlatformApplicationModel
    {
        public OSType OperatingSystem { get; set; }
        public string ExecutablePath { get; set; }
    }
}
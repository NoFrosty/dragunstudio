using Evorine.Manager;

namespace Evorine.API.Models
{
    public class AddPlatformApplicationModel
    {
        public OSType OperatingSystem { get; set; }
        public string ExecutablePath { get; set; }
    }
}
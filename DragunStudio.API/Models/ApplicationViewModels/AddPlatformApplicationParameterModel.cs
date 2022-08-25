using DragunStudio.Managers.Package.Enums;

namespace DragunStudio.API.Models.ApplicationViewModels
{
    public class AddPlatformApplicationParameterModel
    {
        public OSType OperatingSystem { get; set; }
        public string ExecutablePath { get; set; }
    }
}

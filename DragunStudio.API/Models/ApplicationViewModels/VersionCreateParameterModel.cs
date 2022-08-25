using DragunStudio.Managers.Package.Enums;

namespace DragunStudio.API.Models.ApplicationViewModels
{
    public class VersionCreateParameterModel
    {
        public string NewVersion { get; set; }

        public OSType OperatingSystem { get; set; }

        public IFormFile ZipStream { get; set; }
    }
}

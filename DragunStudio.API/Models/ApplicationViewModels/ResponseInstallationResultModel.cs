using DragunStudio.Managers.Package.Models;

namespace DragunStudio.API.Models.ApplicationViewModels
{
    public class ResponseInstallationResultModel
    {
        public Guid VersionId { get; set; }
        public string Version { get; set; }
        public IEnumerable<PackageFileInfo> Files { get; set; }
    }
}

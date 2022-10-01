using Dapper.Contrib.Extensions;
using DragunStudio.Managers.Package.Enums;

namespace DragunStudio.Managers.Package.Models
{
    [Table("Version")]
    public class VersionInfo
    {
        [ExplicitKey]
        public Guid VersionId { get; set; }

        public Guid ApplicationId { get; set; }
        public string Version { get; set; }
        public int VersionOrder { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool IsReleased { get; set; }
        public OSType OperatingSystem { get; set; }
    }
}
using Dapper.Contrib.Extensions;
using DragunStudio.Managers.Common;

namespace DragunStudio.Managers.Package.Models
{
    [Table("ApplicationPlatform")]
    public class PlatformInfo
    {
        public Guid ApplicationId { get; set; }
        public OSType OperatingSystem { get; set; }
        public string ExecutablePath { get; set; }
    }
}
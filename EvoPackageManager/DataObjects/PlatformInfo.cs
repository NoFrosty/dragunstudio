using Dapper.Contrib.Extensions;
using System;

namespace Evorine.EvoPackageManager.DataObjects
{
    [Table("ApplicationPlatform")]
    public class PlatformInfo
    {
        public Guid ApplicationId { get; set; }
        public OSType OperatingSystem { get; set; }
        public string ExecutablePath { get; set; }
    }
}
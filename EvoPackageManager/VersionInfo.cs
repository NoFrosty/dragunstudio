using Dapper.Contrib.Extensions;
using System;

namespace Evorine.EvoPackageManager
{
    [Table("Version")]
    public class VersionInfo
    {
        [ExplicitKey]
        public string VersionId { get; set; }
        public string ApplicationId { get; set; }
        public string Version { get; set; }
        public int VersionOrder { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool IsReleased { get; set; }
        public OSType OperatingSystem { get; set; }
    }
}
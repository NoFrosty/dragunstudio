using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

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

using Dapper.Contrib.Extensions;
using System;

namespace Evorine.EvoPackageManager
{
    [Table("Application")]
    public class ApplicationInfo
    {
        [ExplicitKey]
        public Guid ApplicationId { get; set; }
        public string Name { get; set; }
        public bool IsPublished { get; set; }
        public bool InMaintenance { get; set; }
        public OSType SupportedOS { get; set; }
        public bool IsVisible { get; set; }
    }
}
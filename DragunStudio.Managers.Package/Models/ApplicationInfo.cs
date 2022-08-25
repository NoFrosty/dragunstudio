using Dapper.Contrib.Extensions;

namespace DragunStudio.Managers.Package.Models
{
    [Table("Application")]
    public class ApplicationInfo
    {
        [ExplicitKey]
        public Guid ApplicationId { get; set; }

        public string Name { get; set; }
        public bool IsPublished { get; set; }
        public bool InMaintenance { get; set; }
        public bool IsVisible { get; set; }
        public string Description { get; set; }
    }
}
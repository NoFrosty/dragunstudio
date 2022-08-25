using Dapper.Contrib.Extensions;

namespace DragunStudio.Managers.Package.Models
{
    [Table("Update")]
    public class Update
    {
        public Guid UpdateId { get; set; }
        public Guid? FromVersionId { get; set; }
        public Guid ToVersionId { get; set; }
        public long DownloadSize { get; set; }
        public long InstallationSize { get; set; }
    }
}
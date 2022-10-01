using Dapper.Contrib.Extensions;

namespace DragunStudio.Managers.Package.Models
{
    [Table("UpdateActionUpdateFile")]
    public class UpdateActionUpdateFile
    {
        [ExplicitKey]
        public Guid ActionId { get; set; }

        public Guid OldFileId { get; set; }
        public Guid NewFileId { get; set; }
        public Guid DiffId { get; set; }
        public long DiffSize { get; set; }
    }
}
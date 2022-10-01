using Dapper.Contrib.Extensions;

namespace DragunStudio.Managers.Package.Models
{
    [Table("UpdateActionDeleteFile")]
    public class UpdateActionDeleteFile
    {
        [ExplicitKey]
        public Guid ActionId { get; set; }

        public Guid FileId { get; set; }
    }
}
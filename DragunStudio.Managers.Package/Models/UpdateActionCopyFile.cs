using Dapper.Contrib.Extensions;
using System;

namespace DragunStudio.Managers.Package.Models
{
    [Table("UpdateActionCopyFile")]
    public class UpdateActionCopyFile
    {
        [ExplicitKey]
        public Guid ActionId { get; set; }

        public Guid SourceFileId { get; set; }
        public Guid DestinationFileId { get; set; }
    }
}
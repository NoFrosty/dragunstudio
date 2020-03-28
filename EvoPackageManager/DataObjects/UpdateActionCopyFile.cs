using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager.DataObjects
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

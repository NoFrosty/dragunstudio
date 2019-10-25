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
        public string ActionId { get; set; }
        public string SourceFileId { get; set; }
        public string DestinationFileId { get; set; }
    }
}

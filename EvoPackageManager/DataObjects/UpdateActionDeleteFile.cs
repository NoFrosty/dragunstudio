using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager.DataObjects
{
    [Table("UpdateActionDeleteFile")]
    public class UpdateActionDeleteFile
    {
        [ExplicitKey]
        public string ActionId { get; set; }
        public string FileId { get; set; }
    }
}

using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager.DataObjects
{
    [Table("UpdateActionCreateFile")]
    public class UpdateActionCreateFile
    {
        [ExplicitKey]
        public string ActionId { get; set; }
        public string FileId { get; set; }
    }
}

using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager.DataObjects
{
    [Table("UpdateActionUpdateFile")]
    public class UpdateActionUpdateFile
    {
        [ExplicitKey]
        public string ActionId { get; set; }
        public string OldFileId { get; set; }
        public string NewFileId { get; set; }
        public string DiffId { get; set; }
    }
}

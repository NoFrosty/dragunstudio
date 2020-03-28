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
        public Guid ActionId { get; set; }
        public Guid OldFileId { get; set; }
        public Guid NewFileId { get; set; }
        public Guid DiffId { get; set; }
        public long DiffSize { get; set; }
    }
}

using Dapper.Contrib.Extensions;
using System;

namespace Evorine.Manager.DataObjects
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
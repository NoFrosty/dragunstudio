using Dapper.Contrib.Extensions;
using System;

namespace Evorine.Manager.DataObjects
{
    [Table("UpdateActionDeleteFile")]
    public class UpdateActionDeleteFile
    {
        [ExplicitKey]
        public Guid ActionId { get; set; }

        public Guid FileId { get; set; }
    }
}
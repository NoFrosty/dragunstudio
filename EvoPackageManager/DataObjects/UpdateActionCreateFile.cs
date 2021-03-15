using Dapper.Contrib.Extensions;
using System;

namespace Evorine.EvoPackageManager.DataObjects
{
    [Table("UpdateActionCreateFile")]
    public class UpdateActionCreateFile
    {
        [ExplicitKey]
        public Guid ActionId { get; set; }

        public Guid FileId { get; set; }
    }
}
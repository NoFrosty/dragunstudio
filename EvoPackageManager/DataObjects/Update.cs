using Dapper.Contrib.Extensions;
using System;

namespace Evorine.EvoPackageManager.DataObjects
{
    [Table("Updates")]
    public class Updates
    {
        public Guid UpdateId { get; set; }
        public Guid? FromVersionId { get; set; }
        public Guid ToVersionId { get; set; }
        public long Size { get; set; }
    }
}
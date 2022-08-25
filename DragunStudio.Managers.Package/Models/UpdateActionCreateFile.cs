using Dapper.Contrib.Extensions;
using System;

namespace DragunStudio.Managers.Package.Models
{
    [Table("UpdateActionCreateFile")]
    public class UpdateActionCreateFile
    {
        [ExplicitKey]
        public Guid ActionId { get; set; }

        public Guid FileId { get; set; }
    }
}
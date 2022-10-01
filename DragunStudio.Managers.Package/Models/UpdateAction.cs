using Dapper.Contrib.Extensions;
using DragunStudio.Managers.Package.Enums;

namespace DragunStudio.Managers.Package.Models
{
    [Table("UpdateAction")]
    public class UpdateAction
    {
        [ExplicitKey]
        public Guid ActionId { get; set; }

        public Guid UpdateId { get; set; }
        public ActionType ActionType { get; set; }
        public int ActionOrder { get; set; }
    }
}
using Dapper.Contrib.Extensions;
using System;

namespace Evorine.Manager.DataObjects
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
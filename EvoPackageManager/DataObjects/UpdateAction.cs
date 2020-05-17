using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager.DataObjects
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

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
        public string ActionId { get; set; }
        public string FromVersionId { get; set; }
        public string ToVersionId { get; set; }
        public ActionType ActionType { get; set; }
        public int ActionOrder { get; set; }
    }
}

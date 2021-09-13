using Dapper.Contrib.Extensions;
using System;

namespace Evorine.Manager.Issues.DataObjects
{
    [Table("StepsInfo")]
    class IssueStepInfo
    {
        public string Step { get; set; }

        //public Image ScreenShot { get; set; }
    }
}

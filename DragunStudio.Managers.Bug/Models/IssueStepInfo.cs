using Dapper.Contrib.Extensions;
using System;

namespace DragunStudio.Managers.Bug.Models
{
    [Table("StepsInfo")]
    class IssueStepInfo
    {
        public string Step { get; set; }

        //public Image ScreenShot { get; set; }
    }
}

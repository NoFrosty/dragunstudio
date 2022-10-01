using Dapper.Contrib.Extensions;

namespace DragunStudio.Managers.Issue.Models
{
    [Table("StepsInfo")]
    public class IssueStepInfo
    {
        public string Step { get; set; }

        //public Image ScreenShot { get; set; }
    }
}

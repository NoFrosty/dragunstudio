using DragunStudio.Managers.Issue.Enums;

namespace DragunStudio.Managers.Issue.Models
{
    public class IssueBug : IssueInfo
    {
        public string DescriptionShouldHappen { get; set; }
        public SeverityType Severity { get; set; }
        public List<IssueStepInfo> Steps { get; set; }

    }
}

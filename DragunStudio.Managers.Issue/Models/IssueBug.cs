using DragunStudio.Managers.Issue.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragunStudio.Managers.Issue.Models
{
    public class IssueBug : Issue
    {
        public string DescriptionShouldHappen { get; set; }
        public SeverityType Severity { get; set; }
        public List<IssueStepInfo> Steps { get; set; }

    }
}

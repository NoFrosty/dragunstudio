using Dapper.Contrib.Extensions;
using DragunStudio.Managers.Bug.Enums;
using System;
using System.Collections.Generic;

namespace DragunStudio.Managers.Bug.Models
{
    [Table("Bug")]
    class IssueInfo
    {
        [ExplicitKey]
        public Guid IssueId { get; set; }

        public bool Solved { get; set; }

        public bool Verified { get; set; }

        public int Votes { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string DescriptionShouldHappen { get; set; }

        //Images

        public Guid ReporterUserId { get; set; }

        public SeverityType Severity { get; set; }

        public Guid GameVersionId { get; set; }

        public IssueCategory Category { get; set; }

        public List<IssueStepInfo> Steps { get; set; }

        public List<IssueComment> Comments { get; set; }

    }
}

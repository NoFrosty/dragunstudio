using Dapper.Contrib.Extensions;
using DragunStudio.Managers.Issue.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragunStudio.Managers.Issue.Models
{
    [Table("Issue")]
    public class IssueInfo
    {
        [ExplicitKey]
        public Guid IssueId { get; set; }

        public Guid ReporterUserId { get; set; }

        public Guid ApplicationVersionId { get; set; }

        public IssueCategory Category { get; set; }

        public IssueStatus Status { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<IssueComment>? Comments { get; set; }

        public List<IssueVote>? Votes { get; set; }

    }
}

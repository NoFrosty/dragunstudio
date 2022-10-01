using Dapper.Contrib.Extensions;
using DragunStudio.Managers.Issue.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragunStudio.Managers.Issue.Models
{
    [Table("Votes")]
    public class IssueVote
    {
        [ExplicitKey]
        public Guid UserId { get; set; }

        public Guid IssueId { get; set; }

        public DateTime Date { get; set; }

        public VoteType VoteType { get; set; }
    }
}

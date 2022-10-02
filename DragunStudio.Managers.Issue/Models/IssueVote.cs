using Dapper.Contrib.Extensions;
using DragunStudio.Managers.Issue.Enums;

namespace DragunStudio.Managers.Issue.Models
{
    [Table("Votes")]
    public class IssueVote
    {
        [ExplicitKey]
        public Guid VoteId { get; set; }

        public Guid UserId { get; set; }

        public Guid IssueId { get; set; }

        public DateTime Date { get; set; }

        public VoteType VoteType { get; set; }
    }
}

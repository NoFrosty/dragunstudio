using Dapper.Contrib.Extensions;

namespace DragunStudio.Managers.Issue.Models
{
    [Table("Comments")]
    public class IssueComment
    {
        [ExplicitKey]
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }

        public Guid IssueId { get; set; }

        public string Comment { get; set; }

        public DateTime Date { get; set; }

        public bool CanReproduce { get; set; }
    }
}

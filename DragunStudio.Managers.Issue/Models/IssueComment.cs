using Dapper.Contrib.Extensions;
using System;

namespace DragunStudio.Managers.Issue.Models
{
    [Table("Comments")]
    public class IssueComment
    {
        [ExplicitKey]
        public Guid UserId { get; set; }

        public Guid IssueId { get; set; }

        public string Comment { get; set; }

        public DateTime Date { get; set; }

        public bool CanReproduce { get; set; }
    }
}

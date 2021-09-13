using Dapper.Contrib.Extensions;
using System;

namespace Evorine.Manager.Issues.DataObjects
{
    [Table("Comments")]
    class IssueComment
    {
        public Guid UserId { get; set; }

        public string Comment { get; set; }

        public DateTime Date { get; set; }

        public bool CanReproduce { get; set; }
    }
}

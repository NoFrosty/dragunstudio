using Dapper.Contrib.Extensions;
using System;

namespace DragunStudio.Managers.Bug.Models
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

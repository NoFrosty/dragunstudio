using Dapper.Contrib.Extensions;
using DragunStudio.Common;
using DragunStudio.Managers.Issue.Models;

namespace DragunStudio.Managers.Issue
{
    class DataAccess
    {
        private readonly ConnectionFactory connectionFactory;

        public DataAccess(ConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async virtual Task AddIssue(IssueInfo issueInfo)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(issueInfo);
            }
        }

        public async Task<IssueInfo> GetIssueInfoAsync(Guid issueInfoId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.GetAsync<IssueInfo>(issueInfoId);
            }
        }

        public async Task<IEnumerable<IssueInfo>> GetAllIssueInfosAsync()
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.GetAllAsync<IssueInfo>();
            }
        }
    }
}

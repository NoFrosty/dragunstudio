using Dapper;
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
        public async void InsertComment(IssueComment iComment)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(iComment);
            }
        }

        public async Task<bool> UpdateComment(IssueComment iComment)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.UpdateAsync(iComment);
            }
        }

        public async Task<IssueVote> GetUserVoteForIssueIfExist(Guid issueId, Guid userId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                string sql = "SELECT * FROM IssueVotes WHERE IssueId = @IssueId AND UserId = @UserId";
                return await connection.QueryFirstAsync<IssueVote>(sql, new { IssueId = issueId , UserId = userId });
            }
        }

        public async void InsertVote(IssueVote vote)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(vote);
            }
        }

        public async Task<bool> UpdateVote(IssueVote vote)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.UpdateAsync(vote);
            }
        }
    }
}

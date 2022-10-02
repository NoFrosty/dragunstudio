using crypto;
using DragunStudio.Common;
using DragunStudio.Managers.Issue.Enums;
using DragunStudio.Managers.Issue.Models;
using DragunStudio.Managers.Issue.Models.Game;
using DragunStudio.Managers.Package.Enums;

namespace DragunStudio.Managers.Issue
{
    class IssueManager
    {
        private readonly ApplicationSettings applicationSettings;
        private readonly DataAccess dataAccess;

        public IssueManager(ApplicationSettings applicationSettings, DataAccess dataAccess)
        {
            this.applicationSettings = applicationSettings;
            this.dataAccess = dataAccess;
        }

        public async Task<IssueBugGame> AddIssueBugFromGame(Guid userId, Guid applicationId, IssueCategory category, string title, string description, string descriptionShouldHappen, SeverityType severity, List<string> steps, float positionX, float positionY, float positionZ, OSType OS)
        {
            IssueBugGame issue = new IssueBugGame()
            {
                IssueId = Guid.NewGuid(),
                ReporterUserId = userId,
                ApplicationVersionId = applicationId,
                Category = category,
                Title = title,
                Description = description,
                DescriptionShouldHappen = descriptionShouldHappen,
                Severity = severity,
                Steps = new List<IssueStepInfo>(),
                PlayerPositionX = positionX,
                PlayerPositionY = positionY,
                PlayerPositionZ = positionZ,
                OSPlatform = OS,
                Status = IssueStatus.Open,
                Votes = new List<IssueVote>(),
                Comments = new List<IssueComment>()
            };

            foreach (string step in steps)
            {
                issue.Steps.Add(new IssueStepInfo()
                {
                    Step = step,
                });
            }

            await dataAccess.InsertIssueBugFromGame(issue);

            return issue;
        }

        public async Task<IssueBug> AddIssueBug(Guid userId, Guid applicationId, IssueCategory category, string title, string description, string descriptionShouldHappen, SeverityType severity, List<string> steps)
        {
            IssueBug issue = new IssueBug()
            {
                IssueId = Guid.NewGuid(),
                ReporterUserId = userId,
                ApplicationVersionId = applicationId,
                Category = category,
                Title = title,
                Description = description,
                DescriptionShouldHappen = descriptionShouldHappen,
                Severity = severity,
                Steps = new List<IssueStepInfo>(),
                Status = IssueStatus.Open,
                Votes = new List<IssueVote>(),
                Comments = new List<IssueComment>()
            };

            await dataAccess.InsertIssueBug(issue);

            return issue;
        }

        public async Task<IssueInfo> AddIssue(Guid userId, Guid applicationId, IssueCategory category, string title, string description)
        {
            IssueInfo issue = new IssueInfo()
            {
                IssueId = Guid.NewGuid(),
                ReporterUserId = userId,
                ApplicationVersionId = applicationId,
                Category = category,
                Title = title,
                Description = description,
                Status = IssueStatus.Open,
                Votes = new List<IssueVote>(),
                Comments = new List<IssueComment>()
            };

            await dataAccess.InsertIssue(issue);

            return issue;
        }

        public async Task<IssueVote> InsertVote(Guid issueId, Guid userId, VoteType voteType)
        {

            IssueVote vote = await dataAccess.GetUserVoteForIssueIfExist(issueId, userId);
            if (vote == null)
            {
                vote = new IssueVote()
                {
                    VoteId = Guid.NewGuid(),
                    IssueId = issueId,
                    UserId = userId,
                    Date = DateTime.UtcNow,
                    VoteType = voteType,
                };

                await dataAccess.InsertVote(vote);
            }
            else
            {
                vote.VoteType = voteType;
                vote.Date = DateTime.UtcNow;

                await dataAccess.UpdateVote(vote);
            }
            
            return vote;
        }

        public async Task<IssueComment> InsertComment(Guid issueId, Guid userId, bool canReproduce, string comment)
        {
            IssueComment iComment = new IssueComment()
            {
                CommentId = Guid.NewGuid(),
                UserId = userId,
                CanReproduce = canReproduce,
                Comment = comment,
                IssueId = issueId,
                Date = DateTime.UtcNow
            };

            await dataAccess.InsertComment(iComment);
            //DataAccess

            return iComment;
        }

        public async Task<IssueInfo> GetIssue(Guid issueInfoId)
        {
            return await dataAccess.GetIssueInfoAsync(issueInfoId);
        }

        public async Task<IEnumerable<IssueInfo>> GetAllIssues()
        {
            return await dataAccess.GetAllIssueInfosAsync();
        }
    }
}

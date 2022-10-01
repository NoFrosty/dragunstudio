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

        public async Task<IssueBugGame> AddIssueBugFromGame(Guid userId, Guid applicationId, IssueCategory category, string title, string description, string descriptionShouldHappen, SeverityType severity, List<string> steps, float x, float y, float z, OSType OS)
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
                PlayerPositionX = x,
                PlayerPositionY = y,
                PlayerPositionZ = z,
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

            throw new NotImplementedException();
            //DataAccess

            return issue;
        }

        public async Task<IssueBug> AddIssueBug(Guid userId, Guid applicationId, IssueCategory category, string title, string description, string descriptionShouldHappen, SeverityType severity, List<string> steps)
        {
            IssueBug issue = new IssueBug();

            throw new NotImplementedException();
            //DataAccess

            return issue;
        }

        public async Task<IssueInfo> AddIssue(Guid userId, Guid applicationId, IssueCategory category, string title, string description)
        {
            IssueInfo issue = new IssueInfo();

            throw new NotImplementedException();
            //DataAccess

            return issue;
        }

        public async Task<IssueVote> AddVote(Guid issueId, Guid userId, VoteType voteType)
        {
            IssueVote vote = new IssueVote();

            throw new NotImplementedException();
            //DataAccess

            return vote;
        }

        public async Task<IssueComment> AddComment(Guid issueId, Guid userId, bool canReproduce, string comment)
        {
            IssueComment iComment = new IssueComment();

            throw new NotImplementedException();
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

        public async void AddIssue()
        {

        }
    }
}

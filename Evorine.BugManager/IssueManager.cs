using Evorine.Manager.Issues.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evorine.Manager.Issues
{
    class IssueManager
    {
        private readonly IssueManagerSettings settings;
        private readonly DataAccess dataAccess;

        public IssueManager(IssueManagerSettings settings, DataAccess dataAccess)
        {
            this.settings = settings;
            this.dataAccess = dataAccess;
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

using DragunStudio.Common;
using DragunStudio.Managers.Bug.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragunStudio.Managers.Bug
{
    class BugManager
    {
        private readonly ApplicationSettings applicationSettings;
        private readonly DataAccess dataAccess;

        public BugManager(ApplicationSettings applicationSettings, DataAccess dataAccess)
        {
            this.applicationSettings = applicationSettings;
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

namespace Evorine.Manager.Issues
{
    class ConnectionStringFactory : IConnectionStringFactory
    {
        private readonly IssueManagerSettings issueManagerSettings;

        public ConnectionStringFactory(IssueManagerSettings issueManagerSettings)
        {
            this.issueManagerSettings = issueManagerSettings;
        }

        public string GetConnectionString()
        {
            return issueManagerSettings.ConnectionString;
        }
    }
}

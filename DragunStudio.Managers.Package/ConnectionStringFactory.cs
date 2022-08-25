using DragunStudio.Common;

namespace DragunStudio.Managers.Package
{
    public class ConnectionStringFactory : IConnectionStringFactory
    {
        private readonly ApplicationSettings applicationSettings;

        public ConnectionStringFactory(ApplicationSettings applicationSettings)
        {
            this.applicationSettings = applicationSettings;
        }

        public string GetConnectionString()
        {
            return applicationSettings.ConnectionStringMySQL;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager
{
    public class ConnectionStringFactory : IConnectionStringFactory
    {
        private readonly PackageManagerSettings packageManagerSettings;
        public ConnectionStringFactory(PackageManagerSettings packageManagerSettings)
        {
            this.packageManagerSettings = packageManagerSettings;
        }
        public string GetConnectionString()
        {
            return packageManagerSettings.ConnectionString;
        }
    }
}

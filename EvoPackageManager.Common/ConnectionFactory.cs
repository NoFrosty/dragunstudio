using MySql.Data.MySqlClient;
using System.Data.Common;

namespace Evorine.EvoPackageManager
{
    public class ConnectionFactory
    {
        private readonly IConnectionStringFactory connectionStringFactory;

        public ConnectionFactory(IConnectionStringFactory connectionStringFactory)
        {
            this.connectionStringFactory = connectionStringFactory;
        }

        public DbConnection AcquireConnection()
        {
            return new MySqlConnection(connectionStringFactory.GetConnectionString());
        }
    }
}
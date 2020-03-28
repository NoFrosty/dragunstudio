using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Evorine.EvoPackageManager
{
    public class ConnectionFactory
    {
        private readonly ConnectionStringFactory connectionStringFactory;
        public ConnectionFactory(ConnectionStringFactory connectionStringFactory)
        {
            this.connectionStringFactory = connectionStringFactory;
        }
        public DbConnection AquireConnection()
        {
            return new MySqlConnection(connectionStringFactory.GetConnectionString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Evopeons.API.Data
{
    public class SqliteConnectionFactory : IConnectionFactory
    {
        readonly ApplicationSettings applicationSettings;

        public SqliteConnectionFactory(ApplicationSettings applicationSettings)
        {
            this.applicationSettings = applicationSettings;
        }

        public IDbConnection GetConnection()
        {
            return new SQLiteConnection(applicationSettings.ConnectionString);
        }
    }
}

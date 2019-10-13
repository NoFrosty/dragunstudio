using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Evorine.EvoPackageManager
{
    public class DataAccess
    {
        ConnectionStringFactory connectionStringFactory;
        public DataAccess(ConnectionStringFactory connectionStringFactory)
        {
            this.connectionStringFactory = connectionStringFactory;
        }
        public async virtual Task InsertApplication(ApplicationInfo applicationInfo)
        {
            applicationInfo.ApplicationId = Guid.NewGuid().ToString();
            using(var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                await connection.InsertAsync(applicationInfo);
            }
        }

        public async Task<ApplicationInfo> GetApplication(string applicationId)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                return await connection.GetAsync<ApplicationInfo>(applicationId.ToString());
            }
        }

        public async Task<IEnumerable<ApplicationInfo>> GetApplications()
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                return await connection.GetAllAsync<ApplicationInfo>();
            }
        }

        public async Task<bool> UpdateApplication(ApplicationInfo applicationInfo)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                return await connection.UpdateAsync(applicationInfo);
            }
        }

        public async Task AddVersion(VersionInfo versionInfo)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                await connection.InsertAsync(versionInfo);
            }
        }

        public async Task<IEnumerable<VersionInfo>> GetVersions()
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                return await connection.GetAllAsync<VersionInfo>();
            }
        }

        public async Task<VersionInfo> GetVersion(string applicationId)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                return await connection.GetAsync<VersionInfo>(applicationId);
            }
        }
        
        public async Task AddFiles(IEnumerable<FileInfo> files)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                await connection.InsertAsync(files);
            }
        }
    }
}

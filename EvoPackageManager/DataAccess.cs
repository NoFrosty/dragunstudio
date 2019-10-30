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

        public async Task<IEnumerable<VersionInfo>> GetVersions(string applicationId)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                return await connection.QueryAsync<VersionInfo>(
                    "SELECT * FROM Version WHERE ApplicationId = @ApplicationId",
                    new { ApplicationId = applicationId }
                );
            }
        }

        public async Task<VersionInfo> GetLastVersion(string applicationId)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                return await connection.QueryFirstOrDefaultAsync<VersionInfo>
                    ("SELECT * FROM Version WHERE ApplicationId = @ApplicationId ORDER BY VersionOrder DESC", new { ApplicationId = applicationId });

            }
        }
        
        public async Task AddFiles(IEnumerable<FileInfo> files)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                await connection.InsertAsync(files);
            }
        }

        public async Task<IEnumerable<FileInfo>> GetFiles(string versionId)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                return await connection.QueryAsync<FileInfo>(
                    "SELECT * FROM File WHERE VersionId = @VersionId",
                    new { VersionId = versionId }
                );
            }
        }

        public async Task AddUpdateAction(DataObjects.UpdateAction updateAction)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                await connection.InsertAsync(updateAction);
            }
        }

        public async Task AddUpdateActionCopy(DataObjects.UpdateActionCopyFile updateActionCopyFile)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                await connection.InsertAsync(updateActionCopyFile);
            }
        }

        public async Task AddUpdateActionDelete(DataObjects.UpdateActionDeleteFile updateActionDeleteFile)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                await connection.InsertAsync(updateActionDeleteFile);
            }
        }
        public async Task AddUpdateActionUpdate(DataObjects.UpdateActionUpdateFile updateActionUpdateFile)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                await connection.InsertAsync(updateActionUpdateFile);
            }
        }
        public async Task AddUpdateActionCreate(DataObjects.UpdateActionCreateFile updateActionCreateFile)
        {
            using (var connection = new SQLiteConnection(connectionStringFactory.GetConnectionString()))
            {
                await connection.InsertAsync(updateActionCreateFile);
            }
        }
    }
}

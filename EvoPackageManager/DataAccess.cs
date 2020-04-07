using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using EvoPackageManager.Models;

namespace Evorine.EvoPackageManager
{
    public class DataAccess
    {
        readonly ConnectionFactory connectionFactory;

        public DataAccess(ConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }
        public async virtual Task InsertApplication(ApplicationInfo applicationInfo)
        {
            applicationInfo.ApplicationId = Guid.NewGuid();
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(applicationInfo);
            }
        }

        public async Task<ApplicationInfo> GetApplication(Guid applicationId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.GetAsync<ApplicationInfo>(applicationId);
            }
        }

        public async Task<IEnumerable<ApplicationInfo>> GetApplications(bool includeHidden)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                string sql = includeHidden ? "SELECT * FROM Application" : "SELECT * FROM Application WHERE IsVisible = 1";
                return await connection.QueryAsync<ApplicationInfo>(sql);
            }
        }

        public async Task<bool> UpdateApplication(ApplicationInfo applicationInfo)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.UpdateAsync(applicationInfo);
            }
        }

        public async Task AddVersion(VersionInfo versionInfo)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(versionInfo);
            }
        }

        public async Task<IEnumerable<VersionInfo>> GetVersions()
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.GetAllAsync<VersionInfo>();
            }
        }

        public async Task<IEnumerable<VersionInfo>> GetVersions(Guid applicationId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryAsync<VersionInfo>(
                    "SELECT * FROM Version WHERE ApplicationId = @ApplicationId",
                    new { ApplicationId = applicationId }
                );
            }
        }

        public async Task<VersionInfo> GetVersion(Guid versionId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<VersionInfo>(
                    "SELECT * FROM Version WHERE VersionId = @VersionId",
                    new { VersionId = versionId}
                );
            }
        }

        public async Task<VersionInfo> GetLastVersion(Guid applicationId, OSType operatingSystem)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<VersionInfo>(
                    "SELECT * FROM Version WHERE IsReleased = true AND ApplicationId = @ApplicationId AND OperatingSystem = @OperatingSystem ORDER BY VersionOrder DESC",
                    new { ApplicationId = applicationId, OperatingSystem = operatingSystem }
                );
            }
        }
        
        public async Task AddFiles(IEnumerable<FileInfo> files)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(files);
            }
        }

        public async Task<IEnumerable<FileInfo>> GetFiles(Guid versionId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryAsync<FileInfo>(
                    "SELECT * FROM File WHERE VersionId = @VersionId",
                    new { VersionId = versionId }
                );
            }
        }

        public async Task<FileInfo> GetFile(string fileId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<FileInfo>(
                    "SELECT * FROM File WHERE FileId = @fileId",
                    new { FileId = fileId }
                );
            }
        }

        public async Task<UpdateAction[]> GetUpdateAction(Guid? fromVersionId, Guid toVersionId)
        {
            List<UpdateAction> actions = new List<UpdateAction>();
            using (var connection = connectionFactory.AcquireConnection())
            {
                var allActions = await connection.QueryAsync<DataObjects.UpdateAction>(
                    @"SELECT * FROM UpdateAction UA
                      WHERE ((@FromVersionId IS NULL AND UA.FromVersionId IS NULL) OR (UA.FromVersionId = @FromVersionId)) 
                        AND (UA.ToVersionId = @ToVersionId)",
                    new
                    {
                        FromVersionId = fromVersionId,
                        ToVersionId = toVersionId
                    }
                );

                // Copy Actions
                var copyActions = await connection.QueryAsync<DataObjects.UpdateActionCopyFile>(
                    @"SELECT CP.* FROM UpdateAction UA
                      JOIN UpdateActionCopyFile CP ON CP.ActionId = UA.ActionId
                      WHERE ((@FromVersionId IS NULL AND UA.FromVersionId IS NULL) OR (UA.FromVersionId = @FromVersionId)) 
                        AND (UA.ToVersionId = @ToVersionId)",
                    new
                    {
                        FromVersionId = fromVersionId,
                        ToVersionId = toVersionId
                    }
                );

                // Create Actions
                var createActions = await connection.QueryAsync<DataObjects.UpdateActionCreateFile>(
                    @"SELECT CR.* FROM UpdateAction UA
                      JOIN UpdateActionCreateFile CR ON CR.ActionId = UA.ActionId
                      WHERE ((@FromVersionId IS NULL AND UA.FromVersionId IS NULL) OR (UA.FromVersionId = @FromVersionId)) 
                        AND (UA.ToVersionId = @ToVersionId)",
                    new
                    {
                        FromVersionId = fromVersionId,
                        ToVersionId = toVersionId
                    }
                );

                // Delete Actions
                var deleteActions = await connection.QueryAsync<DataObjects.UpdateActionDeleteFile>(
                    @"SELECT RM.* FROM UpdateAction UA
                      JOIN UpdateActionDeleteFile RM ON RM.ActionId = UA.ActionId
                      WHERE ((@FromVersionId IS NULL AND UA.FromVersionId IS NULL) OR (UA.FromVersionId = @FromVersionId)) 
                        AND (UA.ToVersionId = @ToVersionId)",
                    new
                    {
                        FromVersionId = fromVersionId,
                        ToVersionId = toVersionId
                    }
                );

                // Update Actions
                var updateActions = await connection.QueryAsync<DataObjects.UpdateActionUpdateFile>(
                    @"SELECT UP.* FROM UpdateAction UA
                      JOIN UpdateActionUpdateFile UP ON UP.ActionId = UA.ActionId
                      WHERE ((@FromVersionId IS NULL AND UA.FromVersionId IS NULL) OR (UA.FromVersionId = @FromVersionId)) 
                        AND (UA.ToVersionId = @ToVersionId)",
                    new
                    {
                        FromVersionId = fromVersionId,
                        ToVersionId = toVersionId
                    }
                );

                var tofiles = await GetFiles(toVersionId);
                var fromFiles = fromVersionId != null ? await GetFiles(fromVersionId.Value) : Enumerable.Empty<FileInfo>();
                foreach (var copyAction in copyActions)
                {
                    var action = new Actions.CopyFileAction()
                    {
                        DestinationFile = tofiles.First(x => x.FileId == copyAction.DestinationFileId),
                        SourceFile = fromFiles.First(x => x.FileId == copyAction.SourceFileId)
                    };
                    actions.Add(action);
                }
                foreach (var createAction in createActions)
                {
                    var action = new Actions.CreateFileAction()
                    {
                        NewFile = tofiles.First(x => x.FileId == createAction.FileId)
                    };
                    actions.Add(action);
                }
                foreach (var deleteAction in deleteActions)
                {
                    var action = new Actions.DeleteFileAction()
                    {
                        File = fromFiles.First(x => x.FileId == deleteAction.FileId)
                    };
                    actions.Add(action);
                }
                foreach (var updateAction in updateActions)
                {
                    var action = new Actions.UpdateFileAction()
                    {
                        DiffId = updateAction.DiffId,
                        DiffSize = updateAction.DiffSize,
                        NewFile = tofiles.First(x => x.FileId == updateAction.NewFileId),
                        OldFile = fromFiles.First(x => x.FileId == updateAction.OldFileId)
                    };
                    actions.Add(action);
                }
            }
            return actions.ToArray();
        }

        public async Task AddUpdateAction(DataObjects.UpdateAction updateAction)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(updateAction);
            }
        }

        public async Task AddUpdateActionCopy(DataObjects.UpdateActionCopyFile updateActionCopyFile)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(updateActionCopyFile);
            }
        }
        public async Task<DataObjects.UpdateActionCopyFile> GetUpdateActionCopy(Guid actionId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<DataObjects.UpdateActionCopyFile>(
                    "SELECT * FROM UpdateActionCopyFile WHERE ActionId = @ActionId",
                    new { ActionId = actionId }
                );
            }
        }

        public async Task AddUpdateActionDelete(DataObjects.UpdateActionDeleteFile updateActionDeleteFile)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(updateActionDeleteFile);
            }
        }

        public async Task<DataObjects.UpdateActionDeleteFile> GetUpdateActionDelete(Guid actionId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<DataObjects.UpdateActionDeleteFile>(
                    "SELECT * FROM UpdateActionDeleteFile WHERE ActionId = @ActionId",
                    new { ActionId = actionId }
                );
            }
        }

        public async Task AddUpdateActionUpdate(DataObjects.UpdateActionUpdateFile updateActionUpdateFile)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(updateActionUpdateFile);
            }
        }

        public async Task<DataObjects.UpdateActionUpdateFile> GetUpdateActionUpdate(Guid actionId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<DataObjects.UpdateActionUpdateFile>(
                    "SELECT * FROM UpdateActionUpdateFile WHERE ActionId = @ActionId",
                    new { ActionId = actionId }
                );
            }
        }

        public async Task AddUpdateActionCreate(DataObjects.UpdateActionCreateFile updateActionCreateFile)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(updateActionCreateFile);
            }
        }

        public async Task<DataObjects.UpdateActionCreateFile> GetUpdateActionCreate(Guid actionId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<DataObjects.UpdateActionCreateFile>(
                    "SELECT * FROM UpdateActionDeleteFile WHERE ActionId = @ActionId",
                    new { ActionId = actionId }
                );
            }
        }
    }
}

using Dapper;
using Dapper.Contrib.Extensions;
using Evorine.Manager.DataObjects;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Evorine.Manager
{
    public class DataAccess
    {
        private readonly ConnectionFactory connectionFactory;

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
                string sql = "SELECT * FROM Application WHERE IsDeleted = 0 AND (@IncludeHidden = 1 OR (@IncludeHidden = 0 AND IsVisible = 1))";
                return await connection.QueryAsync<ApplicationInfo>(sql, new { IncludeHidden = includeHidden });
            }
        }

        public async Task<VersionSizeInfo> GetApplicationSizeInfo(Guid applicationId, OSType operatingSystem)
        {
            VersionInfo lastVersionId = await GetLastVersion(applicationId, operatingSystem);
            VersionSizeInfo versionSizeInfo = new VersionSizeInfo();
            using (DbConnection connection = connectionFactory.AcquireConnection())
            {
                string sql = "SELECT InstallationSize FROM Updates WHERE FromVersionId IS NULL AND ToVersionId = @ToVersionId";
                versionSizeInfo.InstallationSize = await connection.QueryFirstOrDefaultAsync<long>(sql, new { ToVersionId = lastVersionId.VersionId });
                sql = "SELECT DownloadSize FROM Updates WHERE FromVersionId IS NULL AND ToVersionId = @ToVersionId";
                versionSizeInfo.DownloadSize = await connection.QueryFirstOrDefaultAsync<long>(sql, new { ToVersionId = lastVersionId.VersionId });
                return versionSizeInfo;
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
                    new { VersionId = versionId }
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

        public async Task AddFiles(IEnumerable<PackageFileInfo> files)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(files);
            }
        }

        public async Task<IEnumerable<PackageFileInfo>> GetFiles(Guid versionId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryAsync<PackageFileInfo>(
                    "SELECT * FROM File WHERE VersionId = @VersionId",
                    new { VersionId = versionId }
                );
            }
        }

        public async Task<PackageFileInfo> GetFile(string fileId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<PackageFileInfo>(
                    "SELECT * FROM File WHERE FileId = @fileId",
                    new { FileId = fileId }
                );
            }
        }

        public async Task<int> DeleteApplication(Guid applicationID)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.ExecuteAsync(
                    "UPDATE Application SET IsDeleted = 1 WHERE ApplicationId = @ApplicationId",
                    new { ApplicationId = applicationID }
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
                var fromFiles = fromVersionId != null ? await GetFiles(fromVersionId.Value) : Enumerable.Empty<PackageFileInfo>();
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

        public async Task UpdateVersion(VersionInfo version)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.UpdateAsync(version);
            }
        }

        public async Task AddPlatformToApplication(PlatformInfo platformInfo)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(platformInfo);
            }
        }

        public async Task<UpdateAction[]> GetUpdateAction(Updates update)
        {
            List<UpdateAction> actions = new List<UpdateAction>();
            using (var connection = connectionFactory.AcquireConnection())
            {
                var allActions = await connection.QueryAsync<DataObjects.UpdateAction>(
                    @"SELECT * FROM UpdateAction UA
                      WHERE UA.UpdateId = @UpdateId",
                    new
                    {
                        UpdateId = update.UpdateId
                    }
                );

                // Copy Actions
                var copyActions = await connection.QueryAsync<DataObjects.UpdateActionCopyFile>(
                    @"SELECT CP.* FROM UpdateAction UA
                      JOIN UpdateActionCopyFile CP ON CP.ActionId = UA.ActionId
                      WHERE UpdateId = @UpdateId",
                    new
                    {
                        UpdateId = update.UpdateId
                    }
                );

                // Create Actions
                var createActions = await connection.QueryAsync<DataObjects.UpdateActionCreateFile>(
                    @"SELECT CR.* FROM UpdateAction UA
                      JOIN UpdateActionCreateFile CR ON CR.ActionId = UA.ActionId
                      WHERE UpdateId = @UpdateId",
                    new
                    {
                        UpdateId = update.UpdateId
                    }
                );

                // Delete Actions
                var deleteActions = await connection.QueryAsync<DataObjects.UpdateActionDeleteFile>(
                    @"SELECT RM.* FROM UpdateAction UA
                      JOIN UpdateActionDeleteFile RM ON RM.ActionId = UA.ActionId
                      WHERE UpdateId = @UpdateId",
                    new
                    {
                        UpdateId = update.UpdateId
                    }
                );

                // Update Actions
                var updateActions = await connection.QueryAsync<DataObjects.UpdateActionUpdateFile>(
                    @"SELECT UP.* FROM UpdateAction UA
                      JOIN UpdateActionUpdateFile UP ON UP.ActionId = UA.ActionId
                      WHERE UpdateId = @UpdateId",
                    new
                    {
                        UpdateId = update.UpdateId
                    }
                );

                var tofiles = await GetFiles(update.ToVersionId);
                var fromFiles = update.FromVersionId != null ? await GetFiles(update.FromVersionId.Value) : Enumerable.Empty<PackageFileInfo>();
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

        public async Task<IEnumerable<PlatformInfo>> GetApplicationPlatforms(Guid applicationId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryAsync<PlatformInfo>(
                    "SELECT * FROM ApplicationPlatform WHERE ApplicationId = @ApplicationId",
                    new { ApplicationId = applicationId }
                );
            }
        }

        public async Task AddUpdate(Updates update)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                await connection.InsertAsync(update);
            }
        }

        public async Task<Updates> GetUpdate(Guid? fromVersionId, Guid toVersionId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                string sql;
                if (!fromVersionId.HasValue)
                    sql = "SELECT * FROM Updates WHERE FromVersionId IS NULL AND ToVersionId = @ToVersionId";
                else
                    sql = "SELECT * FROM Updates WHERE FromVersionId = @FromVersionId AND ToVersionId = @ToVersionId";

                return await connection.QueryFirstOrDefaultAsync<Updates>(
                    sql, new { FromVersionId = fromVersionId, ToVersionId = toVersionId }
                );
            }
        }

        public async Task<IEnumerable<Updates>> GetUpdates(Guid toVersionId)
        {
            using (var connection = connectionFactory.AcquireConnection())
            {
                return await connection.QueryAsync<Updates>(
                    "SELECT * FROM Updates WHERE ToVersionId = @ToVersionId",
                    new { ToVersionId = toVersionId }
                );
            }
        }
    }
}
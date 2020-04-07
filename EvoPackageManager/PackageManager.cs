using EvoPackageManager.Models;
using Evorine.EvoPackageManager.Actions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Evorine.EvoPackageManager
{
    public class PackageManager
    {
        readonly PackageManagerSettings settings;
        readonly DataAccess dataAccess;

        public PackageManager(PackageManagerSettings settings, DataAccess dataAccess)
        {
            this.settings = settings;
            this.dataAccess = dataAccess;
        }

        public async Task<ApplicationInfo> GetApplication(Guid applicationId)
        {
            return await dataAccess.GetApplication(applicationId);
        }

        public async Task<IEnumerable<ApplicationInfo>> GetApplications(bool includeHidden)
        {
            return await dataAccess.GetApplications(includeHidden);
        }

        public async Task<IEnumerable<ApplicationViewModel>> GetExtendedApplicationsData(bool isPublished)
        {
            var applications = (await GetApplications(isPublished));
            var models = applications.Select(x => ConvertToApplicationViewModel(x));
            return models;
        }

        public ApplicationViewModel ConvertToApplicationViewModel(ApplicationInfo applicationInfo)
        {
            return new ApplicationViewModel()
            {
                ApplicationId = applicationInfo.ApplicationId,
                IsPublished = applicationInfo.IsPublished,
                InMaintenance = applicationInfo.InMaintenance,
                IsVisible = applicationInfo.IsVisible,
                Name = applicationInfo.Name,
                SupportedOS = applicationInfo.SupportedOS,
                Description = applicationInfo.Description,
                LastVersions = Enum.GetValues(typeof(OSType))
                                       .Cast<OSType>()
                                       .Select(async o => await GetLastVersion(applicationInfo.ApplicationId, o))
                                       .Select(o => o.Result)
                                       .Where(o => o != null)
                                       .ToArray()
            };
        }

        public Stream GetFileStream(Guid applicationId, string fileId)
        {
            return File.OpenRead(Path.Combine(GetCompressedFilesDirectoryPath(applicationId), fileId));
        }
        public Stream GetDiffStream(Guid applicationId, string diffId)
        {
            return File.OpenRead(Path.Combine(GetUpdatesDirectoryPath(applicationId), diffId));
        }


        public async Task<UpdateAction[]> GetUpdateAction(Guid? fromVersionId, Guid toVersionId)
        {
            return await dataAccess.GetUpdateAction(fromVersionId, toVersionId);
        }

        public async Task<DataObjects.UpdateActionCopyFile> GetUpdateActionCopy(Guid actionId)
        {
            return await dataAccess.GetUpdateActionCopy(actionId);
        }

        public async Task<DataObjects.UpdateActionCreateFile> GetUpdateActionCreate(Guid actionId)
        {
            return await dataAccess.GetUpdateActionCreate(actionId);
        }

        public async Task<DataObjects.UpdateActionDeleteFile> GetUpdateActionDelete(Guid actionId)
        {
            return await dataAccess.GetUpdateActionDelete(actionId);
        }

        public async Task<DataObjects.UpdateActionUpdateFile> GetUpdateActionUpdate(Guid actionId)
        {
            return await dataAccess.GetUpdateActionUpdate(actionId);
        }

        public async Task<ApplicationInfo> CreateApplication(string applicationName, OSType operatingSystems)
        {
            var applicationInfo = new ApplicationInfo()
            {
                Name = applicationName,
                InMaintenance = false,
                IsPublished = false,
                SupportedOS = operatingSystems,
                IsVisible = true
            };
            await dataAccess.InsertApplication(applicationInfo);

            Directory.CreateDirectory(GetCompressedFilesDirectoryPath(applicationInfo.ApplicationId));
            Directory.CreateDirectory(GetPackagesDirectoryPath(applicationInfo.ApplicationId));
            Directory.CreateDirectory(GetUpdatesDirectoryPath(applicationInfo.ApplicationId));

            return applicationInfo;
        }

        public Task DeleteApplication()
        {
            //TODO: Delete application directory
            throw new NotImplementedException();
        }

        public async Task<VersionInfo> GetVersion(Guid versionId)
        {
            return await dataAccess.GetVersion(versionId);
        }

        public async Task<IEnumerable<VersionInfo>> GetVersions(Guid applicationId)
        {
            return await dataAccess.GetVersions(applicationId);
        }

        public async Task<VersionInfo> GetLastVersion(Guid applicationId, OSType operatingSystem)
        {
            return await dataAccess.GetLastVersion(applicationId, operatingSystem);
        }

        public async Task<IEnumerable<FileInfo>> GetFiles(Guid versionId)
        {
            return await dataAccess.GetFiles(versionId);
        }

        public async Task<FileInfo> GetFile(string fileId)
        {
            return await dataAccess.GetFile(fileId);
        }

        public async Task<VersionInfo> CreateVersion(Guid applicationId, string newVersion, OSType operatingSystem, Stream zipStream)
        {
            if (newVersion == null) throw new ArgumentNullException(nameof(newVersion));
            if (zipStream == null) throw new ArgumentNullException(nameof(zipStream));

            ApplicationInfo applicationInfo = await GetApplication(applicationId);
            if (applicationInfo == null) throw new ArgumentNullException(nameof(applicationId));
            IEnumerable<VersionInfo> currentVersions = await GetVersions(applicationId);
            if (currentVersions != null && currentVersions.Any(x => x.Version.Equals(newVersion, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"A release with version '{newVersion}' already exists!", nameof(newVersion));
            }

            var fromVersion = currentVersions.SingleOrDefault(x => x.VersionOrder == currentVersions.Max(y => y.VersionOrder));

            VersionInfo newVersionInfo = new VersionInfo()
            {
                VersionId = Guid.NewGuid(),
                Version = newVersion,
                ApplicationId = applicationId,
                IsReleased = false,
                VersionOrder = (fromVersion?.VersionOrder ?? 0) + 1,
                ReleaseDate = null,
                OperatingSystem = operatingSystem
            };

            var versionFiles = StoreVersionFiles(applicationId, newVersionInfo, zipStream);

            await dataAccess.AddVersion(newVersionInfo);
            await dataAccess.AddFiles(versionFiles);
            UpdateManifest updateManifest = await CalculateUpdate(applicationInfo, fromVersion, newVersionInfo);
            UpdateManifest freshInstallationManifest = await CalculateUpdate(applicationInfo, null, newVersionInfo);
            await StoreUpdateManifest(updateManifest);
            await StoreUpdateManifest(freshInstallationManifest);
            return newVersionInfo;
        }

        private List<FileInfo> StoreVersionFiles(Guid applicationId, VersionInfo versionInfo, Stream zipStream)
        {
            List<FileInfo> files = new List<FileInfo>();

            string versionDirectoryPath = Path.Combine(GetPackagesDirectoryPath(applicationId), versionInfo.VersionId.ToString());
            Directory.CreateDirectory(versionDirectoryPath);

            using (ZipArchive archive = new ZipArchive(zipStream))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (!entry.FullName.Last().Equals('/'))
                    {
                        string absoluteFilePath = Path.Combine(versionDirectoryPath, entry.FullName);
                        string absoluteDirectoryPath = Path.GetDirectoryName(absoluteFilePath);

                        // Makes sure directory exists & extracts file
                        Directory.CreateDirectory(absoluteDirectoryPath);
                        entry.ExtractToFile(absoluteFilePath);

                        FileInfo file = new FileInfo()
                        {
                            VersionId = versionInfo.VersionId,
                            FileId = Guid.NewGuid(),
                            FileName = entry.Name,
                            DirectoryPath = Utilities.MakeRelative(absoluteDirectoryPath, versionDirectoryPath),
                            Size = entry.Length
                        };

                        using (Stream fileStream = File.OpenRead(absoluteFilePath))
                        {
                            file.Hash = Utilities.CalculateHash(fileStream);
                        }

                        // Zip file back alone
                        string singleZipPath = Path.Combine(GetCompressedFilesDirectoryPath(applicationId), file.FileId.ToString());
                        using (Stream singleZipFileStream = File.Create(singleZipPath))
                        {
                            using (ZipArchive singleArchive = new ZipArchive(singleZipFileStream, ZipArchiveMode.Create, true))
                            {
                                singleArchive.CreateEntryFromFile(absoluteFilePath, entry.FullName);
                            }
                            file.CompressedSize = singleZipFileStream.Length;
                        }

                        files.Add(file);
                    }
                }
            }

            return files;
        }

        public async Task<UpdateManifest> CalculateUpdate(ApplicationInfo application, VersionInfo versionFrom, VersionInfo versionTo)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));
            if (versionTo == null)
                throw new ArgumentNullException(nameof(versionTo));

            IEnumerable<FileInfo> fromFiles = versionFrom == null ? Enumerable.Empty<FileInfo>() : await GetFiles(versionFrom.VersionId);
            IEnumerable<FileInfo> toFiles = await GetFiles(versionTo.VersionId);

            var fileActions = CalculateFileActions(application.ApplicationId, fromFiles, toFiles);

            var updateManifest = new UpdateManifest()
            {
                ApplicationId = application.ApplicationId,
                FromVersion = versionFrom?.Version,
                FromVersionId = versionFrom?.VersionId,
                ToVersion = versionTo.Version,
                ToVersionId = versionTo.VersionId,
                Actions = fileActions.ToArray()
            };
            return updateManifest;
        }

        private async Task StoreUpdateManifest(UpdateManifest updateManifest)
        {
            for (var i = 0; i < updateManifest.Actions.Length; i++)
            {
                var updateAction = new DataObjects.UpdateAction()
                {
                    ActionId = Guid.NewGuid(),
                    FromVersionId = updateManifest.FromVersionId,
                    ToVersionId = updateManifest.ToVersionId,
                    ActionType = updateManifest.Actions[i].ActionType,
                    ActionOrder = i
                };
                await dataAccess.AddUpdateAction(updateAction);
                switch (updateManifest.Actions[i])
                {
                    case CopyFileAction action:
                        var copyDO = new DataObjects.UpdateActionCopyFile()
                        {
                            ActionId = updateAction.ActionId,
                            SourceFileId = action.SourceFile.FileId,
                            DestinationFileId = action.DestinationFile.FileId
                        };
                        await dataAccess.AddUpdateActionCopy(copyDO);
                        break;
                    case CreateFileAction action:
                        var createDO = new DataObjects.UpdateActionCreateFile()
                        {
                            ActionId = updateAction.ActionId,
                            FileId = action.NewFile.FileId
                        };
                        await dataAccess.AddUpdateActionCreate(createDO);
                        break;
                    case DeleteFileAction action:
                        var deleteDO = new DataObjects.UpdateActionDeleteFile()
                        {
                            ActionId = updateAction.ActionId,
                            FileId = action.File.FileId
                        };
                        await dataAccess.AddUpdateActionDelete(deleteDO);
                        break;
                    case UpdateFileAction action:
                        var updateDO = new DataObjects.UpdateActionUpdateFile()
                        {
                            ActionId = updateAction.ActionId,
                            OldFileId = action.OldFile.FileId,
                            NewFileId = action.NewFile.FileId,
                            DiffId = action.DiffId,
                            DiffSize = action.DiffSize
                        };
                        await dataAccess.AddUpdateActionUpdate(updateDO);
                        break;
                    default:
                        throw new NotImplementedException($"Not implemented kind of action: '{updateManifest.Actions[i].GetType()}'");
                }
            }
        }

        public IList<UpdateAction> CalculateFileActions(Guid applicationId, IEnumerable<FileInfo> fromFiles, IEnumerable<FileInfo> toFiles)
        {
            var copiedFiles = FileInfoUtilities.GetCopyFiles(fromFiles, toFiles);
            var newFiles = FileInfoUtilities.GetNewFiles(fromFiles, toFiles);
            var deletedFiles = FileInfoUtilities.GetDeletedFiles(fromFiles, toFiles);
            var updateFiles = FileInfoUtilities.GetUpdateFiles(fromFiles, toFiles);

            var actions = new List<UpdateAction>();

            foreach (var file in newFiles)
            {
                var action = new CreateFileAction()
                {
                    NewFile = file
                };
                actions.Add(action);
            }
            foreach (var copyGroup in copiedFiles)
            {
                foreach (var copy in copyGroup.Value)
                {
                    var action = new CopyFileAction()
                    {
                        SourceFile = copyGroup.Key,
                        DestinationFile = copy
                    };
                    actions.Add(action);
                }
                // Optimization: Delete in advance if file is already marked to delete
                if (deletedFiles.Contains(copyGroup.Key))
                {
                    var action = new DeleteFileAction()
                    {
                        File = copyGroup.Key
                    };
                    actions.Add(action);
                    deletedFiles = deletedFiles.Where(x => !x.Equals(copyGroup.Key));
                }
            }
            foreach (var file in updateFiles)
            {
                var diffInfo = CreateDiffFiles(applicationId, file.source, file.target);
                var action = new UpdateFileAction()
                {
                    OldFile = file.source,
                    NewFile = file.target,
                    DiffId = diffInfo.diffId,
                    DiffSize = diffInfo.diffSize
                };
                actions.Add(action);
            }
            foreach (var file in deletedFiles)
            {
                var action = new DeleteFileAction()
                {
                    File = file
                };
                actions.Add(action);
            }
            return actions;
        }

        public (Guid diffId, long diffSize) CreateDiffFiles(Guid applicationId, FileInfo oldFile, FileInfo newFile)
        {
            var oldBuffer = File.ReadAllBytes(GetPackagesFilePath(applicationId, oldFile));
            var newBuffer = File.ReadAllBytes(GetPackagesFilePath(applicationId, newFile));
            var diffId = Guid.NewGuid();
            var path = Path.Combine(GetUpdatesDirectoryPath(applicationId), diffId.ToString());

            using (FileStream outputFile = File.Create(path))
            {
                BinaryPatchUtility.Create(oldBuffer, newBuffer, outputFile);
            }
            var fi = new System.IO.FileInfo(path);

            return (diffId, fi.Length);
        }
        public Task<IEnumerable<FileInfo>> GetVersionFiles(Guid versionId)
        {
            return dataAccess.GetFiles(versionId);
        }

        public void DeleteVersion(Guid versionId)
        {
            throw new NotImplementedException();
        }

        public async void UpdateApplication(ApplicationInfo model)
        {
            await dataAccess.UpdateApplication(model);
        }

        public async Task UpdateApplicationMaintenance(Guid applicationId, bool inMaintenance)
        {
            ApplicationInfo applicationInfo = GetApplication(applicationId).Result;
            applicationInfo.InMaintenance = inMaintenance;
            await dataAccess.UpdateApplication(applicationInfo);
        }

        public async Task UpdateApplicationPublishStatus(Guid applicationId, bool isPublished)
        {

            ApplicationInfo applicationInfo = GetApplication(applicationId).Result;
            applicationInfo.IsPublished = isPublished;
            await dataAccess.UpdateApplication(applicationInfo);
        }

        public void UpdateVersionPublishStatus(Guid versionId, bool isPublished)
        {
            throw new NotImplementedException();
        }

        private string GetCompressedFilesDirectoryPath(Guid applicationId)
        {
            return Path.Combine(settings.RootPath, applicationId.ToString(), "CompressedFiles");
        }

        private string GetPackagesDirectoryPath(Guid applicationId)
        {
            return Path.Combine(settings.RootPath, applicationId.ToString(), "Packages");
        }

        private string GetUpdatesDirectoryPath(Guid applicationId)
        {
            return Path.Combine(settings.RootPath, applicationId.ToString(), "Updates");
        }

        private string GetPackagesFilePath(Guid applicationId, FileInfo file)
        {
            return Path.Combine(GetPackagesDirectoryPath(applicationId), file.VersionId.ToString(), file.RelativeFilename);
        }


    }
}

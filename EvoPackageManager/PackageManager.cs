using EvoPackageManager.Models;
using Evorine.EvoPackageManager.Actions;
using Evorine.EvoPackageManager.DataObjects;
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
        private readonly PackageManagerSettings settings;
        private readonly DataAccess dataAccess;

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

        public async Task<VersionSizeInfo> GetApplicationSizeInfo(Guid applicationId, OSType operatingSystem)
        {
            return await dataAccess.GetApplicationSizeInfo(applicationId, operatingSystem);
        }

        public async Task<IEnumerable<ApplicationViewModel>> GetExtendedApplicationsData(bool isPublished)
        {
            var applications = (await GetApplications(isPublished));
            var models = applications.Select(async x => await ConvertToApplicationViewModel(x)).Select(x => x.Result);
            return models;
        }

        public async Task<IEnumerable<Updates>> GetVersionUpdates(Guid versionId)
        {
            return await dataAccess.GetUpdates(versionId);
        }

        public async Task<ApplicationViewModel> ConvertToApplicationViewModel(ApplicationInfo applicationInfo)
        {
            var platforms = (await dataAccess.GetApplicationPlatforms(applicationInfo.ApplicationId)).ToArray();

            return new ApplicationViewModel()
            {
                ApplicationId = applicationInfo.ApplicationId,
                IsPublished = applicationInfo.IsPublished,
                InMaintenance = applicationInfo.InMaintenance,
                IsVisible = applicationInfo.IsVisible,
                Name = applicationInfo.Name,
                Description = applicationInfo.Description,
                Platforms = platforms,
                LastVersions = platforms.Select(async x => await GetLastVersion(applicationInfo.ApplicationId, x.OperatingSystem))
                                        .Select(o => o.Result)
                                        .Where(o => o != null)
                                        .ToArray()
            };
        }

        public async void UpdateApplicationLogo(Guid applicationId, Stream iconStream)
        {
            Directory.CreateDirectory(GetMediaDirectoryPath(applicationId));
            string filePath = GetApplicationLogoPath(applicationId);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await iconStream.CopyToAsync(stream);
            }
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
            return await dataAccess.GetUpdateAction(await dataAccess.GetUpdate(fromVersionId, toVersionId));
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

        public async Task<IEnumerable<Updates>> GetUpdates(Guid versionId)
        {
            return await dataAccess.GetUpdates(versionId);
            throw new NotImplementedException();
        }

        public async Task<DataObjects.UpdateActionDeleteFile> GetUpdateActionDelete(Guid actionId)
        {
            return await dataAccess.GetUpdateActionDelete(actionId);
        }

        public async Task<DataObjects.UpdateActionUpdateFile> GetUpdateActionUpdate(Guid actionId)
        {
            return await dataAccess.GetUpdateActionUpdate(actionId);
        }

        public async Task<ApplicationInfo> CreateApplication(string applicationName)
        {
            var applicationInfo = new ApplicationInfo()
            {
                Name = applicationName,
                InMaintenance = false,
                IsPublished = false,
                IsVisible = true,
                Description = "Lorem Ipsum a la main !",
            };
            await dataAccess.InsertApplication(applicationInfo);

            Directory.CreateDirectory(GetCompressedFilesDirectoryPath(applicationInfo.ApplicationId));
            Directory.CreateDirectory(GetPackagesDirectoryPath(applicationInfo.ApplicationId));
            Directory.CreateDirectory(GetUpdatesDirectoryPath(applicationInfo.ApplicationId));

            return applicationInfo;
        }

        public async Task AddPlatformToApplication(Guid applicationId, OSType operatingSystem, string executablePath)
        {
            PlatformInfo platformInfo = new PlatformInfo()
            {
                ApplicationId = applicationId,
                OperatingSystem = operatingSystem,
                ExecutablePath = executablePath
            };
            await dataAccess.AddPlatformToApplication(platformInfo);
        }

        public async Task DeleteApplication(Guid applicationID)
        {
            //TODO: Delete application directory
            await dataAccess.DeleteApplication(applicationID);
        }

        public async void UpdateVersion(VersionInfo version)
        {
            await dataAccess.UpdateVersion(version);
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

        public async Task<IEnumerable<PackageFileInfo>> GetFiles(Guid versionId)
        {
            return await dataAccess.GetFiles(versionId);
        }

        public async Task<PackageFileInfo> GetFile(string fileId)
        {
            return await dataAccess.GetFile(fileId);
        }

        public async Task<VersionInfo> CreateVersion(Guid applicationId, string newVersion, OSType operatingSystem, string zipPath)
        {
            if (newVersion == null) throw new ArgumentNullException(nameof(newVersion));
            if (zipPath == null) throw new ArgumentNullException(nameof(zipPath));
            if (!File.Exists(zipPath)) throw new FileNotFoundException(nameof(zipPath));


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

            using(Stream stream = File.OpenRead(zipPath))
            {
                var versionFiles = StoreVersionFiles(applicationId, newVersionInfo, stream);
                await dataAccess.AddFiles(versionFiles);
            }

            await dataAccess.AddVersion(newVersionInfo);
            //UpdateManifest updateManifest = await CalculateUpdate(applicationInfo, fromVersion, newVersionInfo);
            UpdateManifest freshInstallationManifest = await CalculateUpdate(applicationInfo, null, newVersionInfo);
            //await StoreUpdateManifest(updateManifest);
            await StoreUpdateManifest(freshInstallationManifest);

            File.Delete(zipPath);

            return newVersionInfo;
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
            //UpdateManifest updateManifest = await CalculateUpdate(applicationInfo, fromVersion, newVersionInfo);
            UpdateManifest freshInstallationManifest = await CalculateUpdate(applicationInfo, null, newVersionInfo);
            //await StoreUpdateManifest(updateManifest);
            await StoreUpdateManifest(freshInstallationManifest);
            return newVersionInfo;
        }

        public async Task<UpdateManifest> CreateVersion(Guid applicationId, Guid fromVersionId, Guid toVersionId)
        {
            ApplicationInfo applicationInfo = await GetApplication(applicationId);
            if (applicationInfo == null) throw new ArgumentNullException(nameof(applicationId));

            if (fromVersionId.Equals(toVersionId)) throw new ArgumentException("Cannot create an update for the same version!");
            Updates update = await dataAccess.GetUpdate(fromVersionId, toVersionId);
            if (update != null) throw new ArgumentException($"An update from version '{fromVersionId}' to version '{toVersionId}' already exists!");
            VersionInfo fromVersionInfo = await dataAccess.GetVersion(fromVersionId);
            if (fromVersionInfo == null) throw new ArgumentNullException(nameof(fromVersionId));
            VersionInfo toVersionInfo = await dataAccess.GetVersion(toVersionId);
            if (toVersionInfo == null) throw new ArgumentNullException(nameof(toVersionInfo));

            UpdateManifest updateManifest = await CalculateUpdate(applicationInfo, fromVersionInfo, toVersionInfo);
            await StoreUpdateManifest(updateManifest);
            return updateManifest;
        }

        private List<PackageFileInfo> StoreVersionFiles(Guid applicationId, VersionInfo versionInfo, Stream zipStream)
        {
            List<PackageFileInfo> files = new List<PackageFileInfo>();

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

                        PackageFileInfo file = new PackageFileInfo()
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
                        string directory = GetCompressedFilesDirectoryPath(applicationId);
                        Directory.CreateDirectory(directory);
                        string singleZipPath = Path.Combine(directory, file.FileId.ToString());
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

            IEnumerable<PackageFileInfo> fromFiles = versionFrom == null ? Enumerable.Empty<PackageFileInfo>() : await GetFiles(versionFrom.VersionId);
            IEnumerable<PackageFileInfo> toFiles = await GetFiles(versionTo.VersionId);

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
            Updates update = new Updates()
            {
                UpdateId = Guid.NewGuid(),
                FromVersionId = updateManifest.FromVersionId,
                ToVersionId = updateManifest.ToVersionId,
                DownloadSize = 0,
                InstallationSize = 0
            };
            for (var i = 0; i < updateManifest.Actions.Length; i++)
            {
                var updateAction = new DataObjects.UpdateAction()
                {
                    ActionId = Guid.NewGuid(),
                    UpdateId = update.UpdateId,
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
                        update.InstallationSize += action.DestinationFile.Size;
                        break;

                    case CreateFileAction action:
                        var createDO = new DataObjects.UpdateActionCreateFile()
                        {
                            ActionId = updateAction.ActionId,
                            FileId = action.NewFile.FileId
                        };
                        update.DownloadSize += action.NewFile.CompressedSize;
                        update.InstallationSize += action.NewFile.Size;
                        await dataAccess.AddUpdateActionCreate(createDO);
                        break;

                    case DeleteFileAction action:
                        var deleteDO = new DataObjects.UpdateActionDeleteFile()
                        {
                            ActionId = updateAction.ActionId,
                            FileId = action.File.FileId
                        };
                        await dataAccess.AddUpdateActionDelete(deleteDO);
                        update.InstallationSize -= action.File.Size;
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
                        update.DownloadSize += action.DiffSize;
                        update.InstallationSize += action.NewFile.Size;
                        await dataAccess.AddUpdateActionUpdate(updateDO);
                        break;

                    default:
                        throw new NotImplementedException($"Not implemented kind of action: '{updateManifest.Actions[i].GetType()}'");
                }
            }
            await dataAccess.AddUpdate(update);
        }

        public IList<UpdateAction> CalculateFileActions(Guid applicationId, IEnumerable<PackageFileInfo> fromFiles, IEnumerable<PackageFileInfo> toFiles)
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

        public (Guid diffId, long diffSize) CreateDiffFiles(Guid applicationId, PackageFileInfo oldFile, PackageFileInfo newFile)
        {
            var oldBuffer = File.ReadAllBytes(GetPackagesFilePath(applicationId, oldFile));
            var newBuffer = File.ReadAllBytes(GetPackagesFilePath(applicationId, newFile));
            var diffId = Guid.NewGuid();
            var directoryPath = GetUpdatesDirectoryPath(applicationId);
            var path = Path.Combine(directoryPath, diffId.ToString());
            Directory.CreateDirectory(directoryPath);
            using (FileStream outputFile = File.Create(path))
            {
                BinaryPatchUtility.Create(oldBuffer, newBuffer, outputFile);
            }
            var fi = new System.IO.FileInfo(path);

            return (diffId, fi.Length);
        }

        public Task<IEnumerable<PackageFileInfo>> GetVersionFiles(Guid versionId)
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

        private string GetMediaDirectoryPath(Guid applicationId)
        {
            return Path.Combine(settings.RootPath, applicationId.ToString(), "Media");
        }

        private string GetPackagesFilePath(Guid applicationId, PackageFileInfo file)
        {
            string path = GetPackagesDirectoryPath(applicationId);
            path = Path.Combine(path, file.VersionId.ToString());
            path = Path.Combine(path, file.RelativeFilename);
            path = Path.Combine(path, file.VersionId.ToString(), file.RelativeFilename);
            return path;
        }

        public string GetApplicationLogoPath(Guid applicationId)
        {
            return Path.Combine(GetMediaDirectoryPath(applicationId), "icon.png");
        }
    }
}
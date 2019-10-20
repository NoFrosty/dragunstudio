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

        public async Task<ApplicationInfo> GetApplication(string applicationId)
        {
            return await dataAccess.GetApplication(applicationId);
        }

        public async Task<IEnumerable<ApplicationInfo>> GetApplications()
        {
            return await dataAccess.GetApplications();
        }

        public async Task<ApplicationInfo> CreateApplication(string applicationName)
        {
            var applicationInfo = new ApplicationInfo()
            { 
                Name = applicationName,
                InMaintenance = false,
                IsPublished = false,
            };
            await dataAccess.InsertApplication(applicationInfo);

            Directory.CreateDirectory(GetCompressedFilesDirectoryPath(applicationInfo.ApplicationId));
            Directory.CreateDirectory(GetPackagesDirectoryPath(applicationInfo.ApplicationId));
            Directory.CreateDirectory(GetUpdatesDirectoryPath(applicationInfo.ApplicationId));

            return applicationInfo;

        }

        public async Task DeleteApplication()
        {
            //TODO: Delete application directory
            throw new NotImplementedException();
        }

        public VersionInfo GetVersion(string applicationId, string versionId)
        {
            throw new NotImplementedException();
        }
        
        public async Task<IEnumerable<VersionInfo>> GetVersions(string applicationId)
        {
            return await dataAccess.GetVersions(applicationId);
        }

        public async Task<VersionInfo> CreateVersion(string applicationId, string version, Stream zipStream)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (zipStream == null) throw new ArgumentNullException(nameof(zipStream));

            ApplicationInfo applicationInfo = await GetApplication(applicationId);
            if(applicationInfo == null) throw new ArgumentNullException(nameof(applicationId));
            IEnumerable<VersionInfo> currentVersions = await GetVersions(applicationId);
            if (currentVersions != null && currentVersions.Any(x => x.Version.Equals(version, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"A release with version '{version}' already exists!", nameof(version));
            }

            var lastVersion = currentVersions.SingleOrDefault(x=>x.VersionOrder == currentVersions.Max(y => y.VersionOrder));

            VersionInfo versionInfo = new VersionInfo()
            {
                VersionId = Guid.NewGuid().ToString(),
                Version = version,
                ApplicationId = applicationId,
                IsReleased = false,
                VersionOrder = (lastVersion?.VersionOrder ?? 0) + 1,
                ReleaseDate = null
            };
            List<FileInfo> files = new List<FileInfo>();

            string versionDirectoryPath = Path.Combine(GetPackagesDirectoryPath(applicationId), versionInfo.VersionId);
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
                            FileId = Guid.NewGuid().ToString(),
                            FileName = entry.Name,
                            DirectoryPath = Utilities.MakeRelative(absoluteDirectoryPath, versionDirectoryPath),
                            Size = entry.Length
                        };

                        using (Stream fileStream = File.OpenRead(absoluteFilePath))
                        {
                            file.Hash = Utilities.CalculateHash(fileStream);
                        }

                        // Zip file back alone
                        string singleZipPath = Path.Combine(GetCompressedFilesDirectoryPath(applicationId), file.FileId);
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
            await dataAccess.AddVersion(versionInfo);
            await dataAccess.AddFiles(files);
            if(currentVersions.Any())
            {
                await CalculateUpdate(applicationInfo, lastVersion, versionInfo);
            }
            return versionInfo;
        }

        public async Task<UpdateManifest> CalculateUpdate(ApplicationInfo application, VersionInfo versionFrom, VersionInfo versionTo)
        {
            IEnumerable<FileInfo> fromFiles = await dataAccess.GetFiles(versionFrom.VersionId);
            IEnumerable<FileInfo> toFiles = await dataAccess.GetFiles(versionTo.VersionId);

            var fileActions = CalculateFileActions(application.ApplicationId, fromFiles, toFiles);

            var updateManifest = new UpdateManifest()
            {
                ApplicationId = application.ApplicationId,
                FromVersion = versionFrom.Version,
                FromVersionId = versionFrom.VersionId,
                ToVersion = versionTo.Version,
                ToVersionId = versionTo.VersionId,
                Actions = fileActions.ToArray()
            };
            return updateManifest;
        }

        public IList<UpdateAction> CalculateFileActions(string applicationId, IEnumerable<FileInfo> fromFiles, IEnumerable<FileInfo> toFiles)
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
                var diffId = CreateDiffFiles(applicationId, file.source, file.target);
                var action = new UpdateFileAction()
                {
                    OldFile = file.source,
                    NewFile = file.target,
                    DiffId = diffId
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

        public string CreateDiffFiles(string applicationId, FileInfo oldFile, FileInfo newFile)
        {
            var oldBuffer = File.ReadAllBytes(GetPackagesFilePath(applicationId, oldFile));
            var newBuffer = File.ReadAllBytes(GetPackagesFilePath(applicationId, newFile));
            var diffId = Guid.NewGuid().ToString();
            var path = Path.Combine(GetUpdatesDirectoryPath(applicationId), diffId);

            using (FileStream outputFile = File.Create(path))
            {
                BinaryPatchUtility.Create(oldBuffer, newBuffer, outputFile);
            }

            return diffId;
        }
        public Task<IEnumerable<FileInfo>> GetVersionFiles(string versionId)
        {
            return dataAccess.GetFiles(versionId);
        }

        public void DeleteVersion(Guid versionId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateApplicationMaintenance(string applicationId, bool inMaintenance)
        {
            ApplicationInfo applicationInfo = GetApplication(applicationId).Result;
            applicationInfo.InMaintenance = inMaintenance;
            await dataAccess.UpdateApplication(applicationInfo);
        }

        public async Task UpdateApplicationPublishStatus(string applicationId, bool isPublished)
        {

            ApplicationInfo applicationInfo = GetApplication(applicationId).Result;
            applicationInfo.IsPublished = isPublished;
            await dataAccess.UpdateApplication(applicationInfo);
        }

        public void UpdateVersionPublishStatus(Guid versionId, bool isPublished)
        {
            throw new NotImplementedException();
        }

        private string GetCompressedFilesDirectoryPath(string applicationId)
        {
            return Path.Combine(settings.RootPath, applicationId, "CompressedFiles");
        }

        private string GetPackagesDirectoryPath(string applicationId)
        {
            return Path.Combine(settings.RootPath, applicationId, "Packages");
        }

        private string GetUpdatesDirectoryPath(string applicationId)
        {
            return Path.Combine(settings.RootPath, applicationId, "Updates");
        }

        private string GetPackagesFilePath(string applicationId, FileInfo file)
        {
            return Path.Combine(GetPackagesDirectoryPath(applicationId), file.VersionId, file.RelativeFilename);
        }


    }
}

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

        public async Task<VersionInfo> GetVersion(string applicationId)
        {
            return await dataAccess.GetVersion(applicationId);
        }

        public async Task<IEnumerable<VersionInfo>> GetVersions()
        {
            return await dataAccess.GetVersions();
        }

        public VersionInfo CreateVersion(string applicationId, string version, Object files)
        {
            throw new NotImplementedException();
        }

        public async Task<VersionInfo> CreateFirstVersion(string applicationId, string version, Stream zipStream)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (zipStream == null) throw new ArgumentNullException(nameof(zipStream));

            ApplicationInfo applicationInfo = await GetApplication(applicationId);
            if(applicationInfo == null) throw new ArgumentNullException(nameof(applicationId));
            VersionInfo checkVersion = await GetVersion(applicationId);
            if (checkVersion != null && checkVersion.Version.Equals(version, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"A release with version '{version}' already exists!", nameof(version));
            }

            VersionInfo versionInfo = new VersionInfo()
            {
                VersionId = Guid.NewGuid().ToString(),
                Version = version,
                ApplicationId = applicationId,
                IsReleased = false,
                VersionOrder = 0
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
                        string filePath = Path.Combine(versionDirectoryPath, entry.FullName);
                        string directoryPath = Path.GetDirectoryName(filePath);

                        Directory.CreateDirectory(directoryPath);

                        entry.ExtractToFile(filePath);

                        FileInfo file = new FileInfo()
                        {
                            VersionId = versionInfo.VersionId,
                            FileId = Guid.NewGuid().ToString(),
                            FileName = entry.Name,
                            DirectoryPath = directoryPath,
                            Size = entry.Length
                        };

                        // Calculate extracted file hash
                        using (Stream fileStream = File.OpenRead(filePath))
                        {
                            file.Hash = Utilities.CalculateHash(fileStream);
                        }

                        // Zip file back alone
                        string singleZipPath = Path.Combine(GetCompressedFilesDirectoryPath(applicationId), file.FileId);
                        using (Stream singleZipFileStream = File.Create(singleZipPath))
                        {
                            using (ZipArchive singleArchive = new ZipArchive(singleZipFileStream, ZipArchiveMode.Create, true))
                            {
                                singleArchive.CreateEntryFromFile(filePath, entry.FullName);
                            }
                            file.CompressedSize = singleZipFileStream.Length;
                        }

                        files.Add(file);
                    }
                }
            }
            await dataAccess.AddVersion(versionInfo);
            await dataAccess.AddFiles(files);
            return versionInfo;
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

    }
}

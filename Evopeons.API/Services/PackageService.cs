using Evopeons.Common;
using Evopeons.Server.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Evopeons.Server.Services
{
    public class PackageService
    {
        readonly AppSettings appSettings;
        readonly DataService dataService;


        public PackageService(IOptions<AppSettings> appSettings, DataService dataService)
        {
            this.appSettings = appSettings.Value;
            this.dataService = dataService;
            CreateFileIfNotExist();
            CheckState();
        }

        public State State { get; private set; }

        public SemVersion GetVersion() => dataService.GetVersion();

        //public SemVersion GetVersion() => dataService.GetVersion();
        public void CreateNewPackage(SemVersion version, Stream zipStream)
        {
            lock (this)
            {
                try
                {
                    // TODO: CHECK IF VERSION ALREADY EXIST

                    if(dataService.GetVersion().Equals(new SemVersion(0, 0)) || dataService.GetVersion()==null)
                    {
                        CreateFirstPackage(version, zipStream);
                    }
                    else
                    {
                        CreateUpdatePackage(dataService.GetVersion(),version, zipStream);
                    }
                }
                catch
                {
                    State = State.PackageNotCreated;
                    throw;
                }
            }
        }

        private void CreateFirstPackage(SemVersion version, Stream packageStream)
        {
            State = State.CreatingPackage;

            UpdateManifest updateManifest = new UpdateManifest()
            {
                FileInfos = new List<Common.FileInfo>(),
                Version = version,
            };


            string path = Path.Combine(appSettings.Files, version.ToString());
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (ZipArchive archive = new ZipArchive(packageStream))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (!entry.FullName.Last().Equals('/'))
                    {
                        string pathFile = Path.Combine(path, entry.FullName);

                        Directory.CreateDirectory(Path.GetDirectoryName(pathFile));

                        entry.ExtractToFile(pathFile); // Can be compressed here

                        Common.FileInfo fileInfo = new Common.FileInfo()
                        {
                            Id = Guid.NewGuid(),
                            FileName = entry.Name,
                            Size = entry.Length, // If Compressed then this need to be the compressed size
                            RelativePath = entry.FullName,
                            PatchAction = PatchAction.Create
                        };
                        using (Stream fileStream = entry.Open())
                        {
                            fileInfo.Hash = Utilities.CalculateHash(fileStream);
                        }
                        updateManifest.FileInfos.Add(fileInfo);
                    }
                }
            }
            CreateZipOfAllFilesInFolder(path);
            File.WriteAllText(Path.Combine(appSettings.Files, version.ToString() + ".json"), JsonConvert.SerializeObject(updateManifest));
            dataService.SetVersion(version);
            State = State.InService;
        }

        private void CreateZipOfAllFilesInFolder(string path)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                using (ZipArchive newFile = ZipFile.Open(Path.GetFileName(file), ZipArchiveMode.Create))
                {
                    newFile.CreateEntryFromFile(Path.GetFileName(file), Path.GetFileName(file), CompressionLevel.Optimal);
                }
            }

        }

        private void CreateUpdatePackage(SemVersion lastVersion, SemVersion newVersion, Stream stream)
        {
            State = State.CreatingPackage;

            UpdateManifest updateManifest = new UpdateManifest()
            {
                FileInfos = new List<Common.FileInfo>(),
                Version = newVersion,
            };

            UpdateManifest lastUpdateManifest = JsonConvert.DeserializeObject<UpdateManifest>(File.ReadAllText(Path.Combine(appSettings.Files, lastVersion.ToString() + ".json")));

            string path = Path.Combine(appSettings.Files, newVersion.ToString());
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (ZipArchive archive = new ZipArchive(stream))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (!entry.FullName.Last().Equals('/'))
                    {
                        string pathFile = Path.Combine(path, entry.FullName);

                        Directory.CreateDirectory(Path.GetDirectoryName(pathFile));

                        entry.ExtractToFile(pathFile); // Can be compressed here

                        Common.FileInfo fileInfo = new Common.FileInfo()
                        {
                            Id = Guid.NewGuid(),
                            FileName = entry.Name,
                            Size = entry.Length,
                            RelativePath = entry.FullName
                        };
                        using (Stream fileStream = entry.Open())
                        {
                            fileInfo.Hash = Utilities.CalculateHash(fileStream);
                        }
                        Common.FileInfo oldFileInfo = lastUpdateManifest.FileInfos.Find(file => file.RelativePath == entry.FullName);
                        if (oldFileInfo != null)
                        {
                            if (oldFileInfo.Hash != fileInfo.Hash)
                                fileInfo.PatchAction = PatchAction.Update;
                            else
                            {
                                if(!oldFileInfo.RelativePath.Equals(fileInfo.RelativePath))
                                    fileInfo.PatchAction = PatchAction.Move;
                                else
                                    fileInfo.PatchAction = PatchAction.None;
                            }
                        }
                        else
                            fileInfo.PatchAction = PatchAction.Create;
                        if (fileInfo.PatchAction == PatchAction.Update)
                        {
                            using (Stream newFileStream = entry.Open())
                            {
                                string p = Path.Combine(appSettings.Updates, lastVersion.ToString() + "-" + newVersion.ToString());
                                Directory.CreateDirectory(p);

                                using (FileStream outputFile = File.Create(Path.Combine(p, entry.Name + ".gzip")))
                                {
                                    using (FileStream oldFileStream = new FileStream(Path.Combine(Path.Combine(appSettings.Files, lastVersion.ToString()), entry.FullName), FileMode.Open))
                                    {
                                        ReadFully(oldFileStream);
                                        ReadFully(oldFileStream);
                                        BinaryPatchUtility.Create(ReadFully(oldFileStream), ReadFully(newFileStream), outputFile);
                                    }
                                }
                            }
                        }
                        updateManifest.FileInfos.Add(fileInfo);
                    }
                }
            }
            IEnumerable<Common.FileInfo> removedFiles = lastUpdateManifest.FileInfos.Where(file => !updateManifest.FileInfos.Any(file2 => file2.FileName == file.FileName));
            if(IsNullOrEmpty(removedFiles))
            {
                foreach (Common.FileInfo file in removedFiles)
                {
                    file.PatchAction = PatchAction.Delete;
                    updateManifest.FileInfos.Add(file);
                }
            }
            dataService.SetVersion(newVersion);
            File.WriteAllText(Path.Combine(appSettings.Files, newVersion.ToString() + ".json"), JsonConvert.SerializeObject(updateManifest));
            State = State.InService;
        }

        public byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public bool IsNullOrEmpty<T>(IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        private void CleanFolder(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
        
        private void CheckState()
        {
            SemVersion version = dataService.GetVersion();

            if (version.Equals(new SemVersion(0, 0)))
            {
                State = State.PackageNotCreated;
            }
            else
            {
                State = State.InService;
            }
        }

        private void CreateFileIfNotExist()
        {
            foreach (string file in Directory.EnumerateFiles(appSettings.Files))
            {
                if (!File.Exists(file))
                    File.Create(file);
            }
        }
    }
}

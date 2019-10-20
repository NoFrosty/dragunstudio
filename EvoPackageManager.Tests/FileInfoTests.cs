using Evorine.EvoPackageManager;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EvoPackageManager.Tests
{
    public class FileInfoTests
    {
        private (IEnumerable<FileInfo>, IEnumerable<FileInfo>) GetDemoFiles()
        {
            var fromFiles = new FileInfo[]
            {
                new FileInfo() { FileName = "Unchanged.txt", DirectoryPath = "Path", Hash = "Hello" },
                new FileInfo() { FileName = "ToBeDeleted.txt", DirectoryPath = "Path", Hash = "delete" },
                new FileInfo() { FileName = "ToBeRenamed.txt", DirectoryPath = "Path", Hash = "rename" },
                new FileInfo() { FileName = "ToBeMoved.txt", DirectoryPath = "Path", Hash = "moved" },
                new FileInfo() { FileName = "ToBeRenamedAndDup.txt", DirectoryPath = "PathDup", Hash = "dup" },
                new FileInfo() { FileName = "Duplicate1.txt", DirectoryPath = "Path", Hash = "dup" },
                new FileInfo() { FileName = "Duplicate2.txt", DirectoryPath = "Path", Hash = "dup" },
                new FileInfo() { FileName = "ToUpdate.txt", DirectoryPath = "Path", Hash = "old" },
            };

            var toFiles = new FileInfo[]
            {
                new FileInfo() { FileName = "Unchanged.txt", DirectoryPath = "Path", Hash = "Hello" },
                // new FileInfo() { FileName = "ToBeDeleted.txt", DirectoryPath = "Path", Hash = "delete" },
                new FileInfo() { FileName = "ToBeRenamed_OLD.txt", DirectoryPath = "Path", Hash = "rename" },
                new FileInfo() { FileName = "ToBeMoved.txt", DirectoryPath = "Path_OLD", Hash = "moved" },
                new FileInfo() { FileName = "ToBeRenamedAndDup_OLD.txt", DirectoryPath = "PathDup", Hash = "dup" },
                new FileInfo() { FileName = "Duplicate1.txt", DirectoryPath = "Path", Hash = "dup" },
                new FileInfo() { FileName = "Duplicate2.txt", DirectoryPath = "Path", Hash = "dup" },
                new FileInfo() { FileName = "New.txt", DirectoryPath = "Path", Hash = "Some Hello!" },
                new FileInfo() { FileName = "Copied.txt", DirectoryPath = "Path", Hash = "dup" },
                new FileInfo() { FileName = "ToUpdate.txt", DirectoryPath = "Path", Hash = "new" },
            };

            return (fromFiles, toFiles);
        }

        [Fact]
        public void FileOperation_GetCopyFiles()
        {
            var (fromFiles, toFiles) = GetDemoFiles();
            var expecteds = new Dictionary<string, string[]>();
            expecteds.Add("ToBeRenamed.txt", new string[] { "ToBeRenamed_OLD.txt" });
            expecteds.Add("ToBeMoved.txt", new string[] { "ToBeMoved.txt" });
            expecteds.Add("ToBeRenamedAndDup.txt", new string[] { "ToBeRenamedAndDup_OLD.txt", "Copied.txt" });

            var result = FileInfoUtilities.GetCopyFiles(fromFiles, toFiles);
            Assert.Equal(result.Keys.Select(x => x.FileName), expecteds.Keys);
            foreach(var copy in expecteds)
            {
                var resultKey = result.Keys.First(x => x.FileName == copy.Key);
                Assert.Equal(copy.Value, result[resultKey].Select(x => x.FileName));
            }
        }

        [Fact]
        public void FileOperation_GetNewFiles()
        {
            var (fromFiles, toFiles) = GetDemoFiles();
            var expected = toFiles
                .Where(x =>
                    x.FileName == "New.txt"
                );

            var result = FileInfoUtilities.GetNewFiles(fromFiles, toFiles);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FileOperation_GetDeletedFiles()
        {
            var (fromFiles, toFiles) = GetDemoFiles();
            var expected = fromFiles
                .Where(x =>
                    x.FileName == "ToBeDeleted.txt" ||
                    x.FileName == "ToBeRenamed.txt" ||
                    x.FileName == "ToBeMoved.txt" ||
                    x.FileName == "ToBeRenamedAndDup.txt"
                );

            var result = FileInfoUtilities.GetDeletedFiles(fromFiles, toFiles);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FileOperation_GetUpdateFiles()
        {
            var (fromFiles, toFiles) = GetDemoFiles();
            var expected = (
                fromFiles.First(x => x.FileName == "ToUpdate.txt"), 
                toFiles.First(x => x.FileName == "ToUpdate.txt"));

            var result = FileInfoUtilities.GetUpdateFiles(fromFiles, toFiles).First();
            Assert.Equal(expected.Item1, result.Item1);
            Assert.Equal(expected.Item2, result.Item2);
        }


        [Fact]
        public void UpdateManifestTest()
        {
            var (fromFiles, toFiles) = GetDemoFiles();
            var packageManager = new PackageManager(null, null);

            //var actions = packageManager.CalculateFileActions(fromFiles, toFiles);
            throw new NotImplementedException("Where are the asserts?");
        }

    }
}

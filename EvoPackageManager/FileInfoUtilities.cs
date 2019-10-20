using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Evorine.EvoPackageManager
{
    public static class FileInfoUtilities
    {
        public static IDictionary<FileInfo, IEnumerable<FileInfo>> GetCopyFiles(IEnumerable<FileInfo> from, IEnumerable<FileInfo> to)
        {
            var sourceFiles = from
                .Where(x => to.Any(t =>
                {
                    return
                        t.Hash.Equals(x.Hash, StringComparison.OrdinalIgnoreCase) &&
                        !t.RelativeFilename.Equals(x.RelativeFilename);
                }))
                .Distinct(FileInfoComparers.Hash);

            return sourceFiles.ToDictionary(
                k => k,
                x => to.Where(
                    t =>
                        t.Hash.Equals(x.Hash, StringComparison.OrdinalIgnoreCase) && !t.RelativeFilename.Equals(x.RelativeFilename) &&
                        !from.Contains(t)
                )
            );
        }

        public static IEnumerable<FileInfo> GetNewFiles(IEnumerable<FileInfo> from, IEnumerable<FileInfo> to)
        {
            var copyFiles = to.Where(x => from.Any(t =>
            {
                return
                    t.Hash.Equals(x.Hash, StringComparison.OrdinalIgnoreCase) &&
                    !t.RelativeFilename.Equals(x.RelativeFilename);
            }));
            return to
                .Where(x => !from.Any(t => t.RelativeFilename.Equals(x.RelativeFilename)))
                .Where(x => !copyFiles.Contains(x));
        }

        public static IEnumerable<FileInfo> GetDeletedFiles(IEnumerable<FileInfo> from, IEnumerable<FileInfo> to)
        {
            return from
                .Where(x => !to.Any(t => t.RelativeFilename.Equals(x.RelativeFilename)));
        }

        public static IEnumerable<(FileInfo source, FileInfo target)> GetUpdateFiles(IEnumerable<FileInfo> from, IEnumerable<FileInfo> to)
        {
            var fromFiles = from.Where(x => to.Any(t => x.RelativeFilename.Equals(t.RelativeFilename) && !x.Hash.Equals(t.Hash)));
            return fromFiles.Select(x => (x, to.Single(t=> x.RelativeFilename.Equals(t.RelativeFilename) && !x.Hash.Equals(t.Hash))));
        }
    }
}

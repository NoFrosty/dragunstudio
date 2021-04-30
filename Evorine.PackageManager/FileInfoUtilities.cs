using Evorine.Manager.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Evorine.Manager
{
    public static class FileInfoUtilities
    {
        public static IDictionary<PackageFileInfo, IEnumerable<PackageFileInfo>> GetCopyFiles(IEnumerable<PackageFileInfo> from, IEnumerable<PackageFileInfo> to)
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

        public static IEnumerable<PackageFileInfo> GetNewFiles(IEnumerable<PackageFileInfo> from, IEnumerable<PackageFileInfo> to)
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

        public static IEnumerable<PackageFileInfo> GetDeletedFiles(IEnumerable<PackageFileInfo> from, IEnumerable<PackageFileInfo> to)
        {
            return from
                .Where(x => !to.Any(t => t.RelativeFilename.Equals(x.RelativeFilename)));
        }

        public static IEnumerable<(PackageFileInfo source, PackageFileInfo target)> GetUpdateFiles(IEnumerable<PackageFileInfo> from, IEnumerable<PackageFileInfo> to)
        {
            var fromFiles = from.Where(x => to.Any(t => x.RelativeFilename.Equals(t.RelativeFilename) && !x.Hash.Equals(t.Hash)));
            return fromFiles.Select(x => (x, to.Single(t => x.RelativeFilename.Equals(t.RelativeFilename) && !x.Hash.Equals(t.Hash))));
        }
    }
}
using Evorine.EvoPackageManager.DataObjects;

namespace Evorine.EvoPackageManager.Actions
{
    public class CopyFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.CopyFile;

        public PackageFileInfo SourceFile { get; set; }
        public PackageFileInfo DestinationFile { get; set; }
    }
}
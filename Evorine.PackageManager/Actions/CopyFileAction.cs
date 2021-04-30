using Evorine.Manager.DataObjects;

namespace Evorine.Manager.Actions
{
    public class CopyFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.CopyFile;

        public PackageFileInfo SourceFile { get; set; }
        public PackageFileInfo DestinationFile { get; set; }
    }
}
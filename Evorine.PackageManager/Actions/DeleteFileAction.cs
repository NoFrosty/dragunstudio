using Evorine.Manager.DataObjects;

namespace Evorine.Manager.Actions
{
    public class DeleteFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.DeleteFile;

        public PackageFileInfo File { get; set; }
    }
}
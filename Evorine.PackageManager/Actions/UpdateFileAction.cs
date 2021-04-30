using Evorine.Manager.DataObjects;
using System;

namespace Evorine.Manager.Actions
{
    public class UpdateFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.UpdateFile;

        public PackageFileInfo OldFile { get; set; }
        public PackageFileInfo NewFile { get; set; }
        public Guid DiffId { get; set; }
        public long DiffSize { get; set; }
    }
}
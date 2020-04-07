using Evorine.EvoPackageManager.DataObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager.Actions
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

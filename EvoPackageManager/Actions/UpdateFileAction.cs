using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager.Actions
{
    public class UpdateFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.UpdateFile;

        public FileInfo OldFile { get; set; }
        public FileInfo NewFile { get; set; }
        public string DiffId { get; set; }
        public long DiffSize { get; set; }
    }
}

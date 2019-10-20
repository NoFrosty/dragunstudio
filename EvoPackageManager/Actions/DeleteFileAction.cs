using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager.Actions
{
    public class DeleteFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.DeleteFile;

        public FileInfo File { get; set; }
    }
}

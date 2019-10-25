using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager.Actions
{
    public class CreateFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.CreateFile;

        public FileInfo NewFile { get; set; }
    }
}

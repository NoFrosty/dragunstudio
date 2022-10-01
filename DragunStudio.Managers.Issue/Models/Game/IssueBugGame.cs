using DragunStudio.Managers.Package.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DragunStudio.Managers.Issue.Models.Game
{
    public class IssueBugGame : IssueBug
    {
        public float PlayerPositionX { get; set; }
        public float PlayerPositionY { get; set; }
        public float PlayerPositionZ { get; set; }
        public string Log { get; set; }
        public OSType OSPlatform { get; set; }
    }
}

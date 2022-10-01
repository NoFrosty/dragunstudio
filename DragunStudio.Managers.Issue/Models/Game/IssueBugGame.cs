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
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string Log { get; set; }
        public OSType OSPlatform { get; set; }
    }
}

using Evopeons.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evopeons.Server.Models
{
    public class Game
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string ExeName { get; set; }
        public int Size { get; set; }
        public SemVersion Version { get; set; }

    }
}

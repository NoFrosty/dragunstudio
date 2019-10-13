using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evopeons.Server.Models
{
    public class AppSettings
    {
        public string CompressedFilesPath { get; set; }
        public string DatabaseFile { get; set; }
        public string ActualVersion { get; set; }
        public string Files { get; set; }
        public string Updates { get; set; }
    }
}

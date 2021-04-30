using Evorine.Manager;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Evorine.API.Models
{
    public class VersionCreateModel
    {
        public string NewVersion { get; set; }

        public OSType OperatingSystem { get; set; }

        public IFormFile ZipStream { get; set; }
    }
}

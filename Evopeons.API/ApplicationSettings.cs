using Evorine.EvoPackageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Evopeons.API
{
    public class ApplicationSettings
    {
        public string ConnectionString { get; set; }
        public string ApplicationSecretKey { get; set; }
        public string AmazonSmtpUsername { get; set; }
        public string AmazonSmtpPassword { get; set; }
        public string AmazonEmailSender { get; set; }
        public string AmazonHost { get; set; }
        public PackageManagerSettings PackageManager { get; set; }
    }
}

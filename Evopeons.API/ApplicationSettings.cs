using Evorine.Manager;

namespace Evorine.API
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
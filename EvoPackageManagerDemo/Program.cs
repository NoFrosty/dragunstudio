using Evorine.EvoPackageManager;
using System;
using System.IO;

namespace EvoPackageManagerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new PackageManagerSettings()
            {
                RootPath = @"C:\Users\Calvi\Desktop\Test"
            };
            ConnectionStringFactory connectionStringFactory = new ConnectionStringFactory();
            var dataAccess = new DataAccess(connectionStringFactory);
            var packageManager = new PackageManager(settings, dataAccess);

            var applicationInfo = packageManager.CreateApplication("Warcraft IV").Result;
            var x = packageManager.GetApplication(applicationInfo.ApplicationId);
            var y = packageManager.GetApplications();
            packageManager.UpdateApplicationPublishStatus("1ac4b846-03f1-4124-9d3e-43afca75b0b5", true).Wait();
            packageManager.UpdateApplicationMaintenance("1ac4b846-03f1-4124-9d3e-43afca75b0b5", true).Wait();
            using (var stream = File.OpenRead(@"C:\Users\Calvi\Downloads\Bagnon-8.1.7.zip"))
            {
                packageManager.CreateFirstVersion(applicationInfo.ApplicationId, "0.0.1", stream).Wait();

            }
        }
    }
}

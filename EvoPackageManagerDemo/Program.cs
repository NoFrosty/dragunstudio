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

            var applicationInfo = packageManager.CreateApplication("Super Sample").Result;
            var x = packageManager.GetApplication(applicationInfo.ApplicationId);
            var y = packageManager.GetApplications();
            packageManager.UpdateApplicationPublishStatus("1ac4b846-03f1-4124-9d3e-43afca75b0b5", true).Wait();
            packageManager.UpdateApplicationMaintenance("1ac4b846-03f1-4124-9d3e-43afca75b0b5", true).Wait();
            using (var stream = File.OpenRead(@"C:\Users\Calvi\Desktop\Test\TestFile\Sample1-V1.zip"))
            {
                packageManager.CreateVersion(applicationInfo.ApplicationId, "v1", stream).Wait();

            }
            using (var stream = File.OpenRead(@"C:\Users\Calvi\Desktop\Test\TestFile\Sample1-V2.zip"))
            {
                packageManager.CreateVersion(applicationInfo.ApplicationId, "v2", stream).Wait();

            }
            /*
            using (var stream = File.OpenRead(@"C:\Users\Calvi\Desktop\Test\TestFile\Sample1-V3.zip"))
            {
                packageManager.CreateVersion(applicationInfo.ApplicationId, "v3", stream).Wait();

            }
            using (var stream = File.OpenRead(@"C:\Users\Calvi\Desktop\Test\TestFile\Sample1-V4.zip"))
            {
                packageManager.CreateVersion(applicationInfo.ApplicationId, "v4", stream).Wait();

            }
            */

        }
    }
}

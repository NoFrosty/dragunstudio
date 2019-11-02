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
                RootPath = @"C:\Users\Calvi\Desktop\Test",
                ConnectionString = @"Data Source=C:\Users\Calvi\Documents\GitHub\Evopeons-Server\Evopeons.API\Evopeons.db;Version=3;"
            };
            ConnectionStringFactory connectionStringFactory = new ConnectionStringFactory(settings);
            var dataAccess = new DataAccess(connectionStringFactory);
            var packageManager = new PackageManager(settings, dataAccess);

            var applicationInfo = packageManager.CreateApplication("Training Station", OSType.Windows).Result;
            using (var stream = File.OpenRead(@"C:\Users\Calvi\Desktop\Test\TestFile\Training Stations.zip"))
            {
                packageManager.CreateVersion(applicationInfo.ApplicationId, "v1", OSType.Windows, stream).Wait();

            }
            /*
            using (var stream = File.OpenRead(@"C:\Users\Calvi\Desktop\Test\TestFile\Sample1-V2.zip"))
            {
                packageManager.CreateVersion(applicationInfo.ApplicationId, "v2", OSType.Windows, stream).Wait();

            }
            
            using (var stream = File.OpenRead(@"C:\Users\Calvi\Desktop\Test\TestFile\Sample1-V3.zip"))
            {
                packageManager.CreateVersion(applicationInfo.ApplicationId, "v3", OSType.Windows, stream).Wait();

            }
            using (var stream = File.OpenRead(@"C:\Users\Calvi\Desktop\Test\TestFile\Sample1-V4.zip"))
            {
                packageManager.CreateVersion(applicationInfo.ApplicationId, "v4", OSType.Windows, stream).Wait();

            }
            */

        }
    }
}

using Evorine.EvoPackageManager;
using System;
using System.IO;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;

namespace EvoPackageManagerDemo
{
    [Table("Testx")]
    public class Testx
    {
        [ExplicitKey]
        public Guid SomeId { get; set; }
        public int SomeNum { get; set; }
        public bool SomethingElse { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var settings = new PackageManagerSettings()
            {
                RootPath = @"C:\Users\Calvi\Desktop\Test",
                ConnectionString = @"Server=192.168.1.55;Database=warp;Uid=warpy;Pwd=112358;"
            };

            ConnectionStringFactory connectionStringFactory = new ConnectionStringFactory(settings);
            ConnectionFactory connectionFactory = new ConnectionFactory(connectionStringFactory);
            var dataAccess = new DataAccess(connectionFactory);
            var packageManager = new PackageManager(settings, dataAccess);

            var applicationInfo = packageManager.CreateApplication("Diable IV", OSType.Windows).Result;
            using (var stream = File.OpenRead(@"C:\Users\Calvi\Desktop\Test\TestFile\MediumPackage.zip"))
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

            }*/
            packageManager.UpdateApplicationPublishStatus(applicationInfo.ApplicationId, true).Wait();
        }
    }
}

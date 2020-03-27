using Evorine.EvoPackageManager;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace EvoPackageManager.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void CreateApplicationMustNotFail()
        {
            var settings = new PackageManagerSettings()
            {
                
            };            
            var dataAccessMock = new Mock<DataAccess>();
            dataAccessMock.Setup(x => x.InsertApplication(It.IsAny<ApplicationInfo>())).Callback<ApplicationInfo>(x => 
            {
                x.ApplicationId = Guid.NewGuid().ToString();
            });

            var packageManager = new PackageManager(settings, dataAccessMock.Object);

            var applicationInfo = packageManager.CreateApplication("Warcraft IV", OSType.Windows).Result;
            
            Assert.NotEqual(default(Guid).ToString(), applicationInfo.ApplicationId);
            Assert.Equal("Warcraft IV", applicationInfo.Name);
            Assert.False(applicationInfo.IsPublished);
            Assert.False(applicationInfo.InMaintenance);
        }
    }
}

using Evorine.EvoPackageManager;
using Evorine.EvoPackageManager.DataObjects;
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
                x.ApplicationId = Guid.NewGuid();
            });

            var packageManager = new PackageManager(settings, dataAccessMock.Object);

            var applicationInfo = packageManager.CreateApplication("Warcraft IV").Result;
            
            Assert.NotEqual(default(Guid), applicationInfo.ApplicationId);
            Assert.Equal("Warcraft IV", applicationInfo.Name);
            Assert.False(applicationInfo.IsPublished);
            Assert.False(applicationInfo.InMaintenance);
        }
    }
}

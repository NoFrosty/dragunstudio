using DragunStudio.API.Models;
using DragunStudio.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DragunStudio.API.Helpers
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        protected readonly IConfiguration configuration;
        private readonly ApplicationSettings applicationSettings;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration, ApplicationSettings applicationSettings)
        : base(options)
        {
            this.configuration = configuration;
            this.applicationSettings = applicationSettings;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to mysql with connection string from app settings
            var connectionString = applicationSettings.ConnectionStringMySQL;
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }
}

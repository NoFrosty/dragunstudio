using Evopeons.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Evopeons.Data
{
    public class EvopeonsDbContext : DbContext
    {
        public EvopeonsDbContext(DbContextOptions<EvopeonsDbContext> options) : base(options)
        {

        }

        public DbSet<News> News { get; set; }
        public DbSet<User> Users { get; set; }
    }
}

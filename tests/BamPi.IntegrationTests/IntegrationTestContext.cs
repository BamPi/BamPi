using System.Data.Entity;

namespace BamPi.IntegrationTests
{
    public class IntegrationTestContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
    }
}
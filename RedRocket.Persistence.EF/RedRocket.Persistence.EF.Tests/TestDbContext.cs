using System.Data.Entity;
using RedRocket.Repositories.EF.Tests.Entities;

namespace RedRocket.Repositories.EF.Tests
{
    public class TestDbContext : DbContext
    {
        public DbSet<Human> Humans { get; set; }
    }
}

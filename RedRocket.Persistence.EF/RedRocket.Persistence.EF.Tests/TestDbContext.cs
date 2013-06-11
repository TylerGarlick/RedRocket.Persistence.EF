using System.Data.Entity;
using FlitBit.IoC;
using FlitBit.IoC.Meta;
using RedRocket.Repositories.EF.Tests.Entities;

namespace RedRocket.Repositories.EF.Tests
{
    [ContainerRegister(ScopeBehavior = ScopeBehavior.InstancePerScope)]
    public class TestDbContext : DbContext
    {
        public DbSet<Human> Humans { get; set; }
    }
}

using System.Data.Entity;
using FlitBit.IoC.Meta;
using RedRocket.Persistence.EF.ContextFactories;

namespace RedRocket.Persistence.EF
{
    [ContainerRegister(typeof(IDbContextFactory), RegistrationBehaviors.Default)]
    public class DefaultDbContextFactory : AbstractInerfaceAwareDbContextFactory, IDbContextFactory
    {
        readonly DbContext _context;

        public DefaultDbContextFactory(DbContext context)
        {
            _context = context;
        }


        public override DbContext GetDbContext<T>(T entity)
        {
            return _context;
        }
    }
}
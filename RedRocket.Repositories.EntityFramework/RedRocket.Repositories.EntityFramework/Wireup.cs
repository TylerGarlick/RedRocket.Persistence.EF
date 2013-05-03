using FlitBit.IoC;
using FlitBit.Wireup;
using FlitBit.Wireup.Meta;
using RedRocket.Repositories.EntityFramework;
using RedRocket.Repositories.EntityFramework.Impl.Repositories;

[assembly: WireupDependency(typeof(WireupThisAssembly))]
[assembly: Wireup(typeof(Wireup))]
namespace RedRocket.Repositories.EntityFramework
{
    public class Wireup : IWireupCommand
    {
        public void Execute(IWireupCoordinator coordinator)
        {
            Container.Root
                     .ForGenericType(typeof(IReadOnlyRepository<>))
                     .Register(typeof(Repository<>))
                     .ResolveAnInstancePerScope()
                     .End();

            Container.Root
                     .ForGenericType(typeof(IRepository<>))
                     .Register(typeof(Repository<>))
                     .ResolveAnInstancePerScope()
                     .End();

        }
    }
}

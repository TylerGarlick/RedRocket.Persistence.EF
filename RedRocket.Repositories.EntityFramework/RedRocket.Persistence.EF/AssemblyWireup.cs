using FlitBit.IoC;
using FlitBit.Wireup;
using FlitBit.Wireup.Meta;
using RedRocket.Persistence.Common;
using RedRocket.Persistence.EF;

[assembly: WireupDependency(typeof(WireupThisAssembly))]
[assembly: Wireup(typeof(AssemblyWireup))]
namespace RedRocket.Persistence.EF
{
    public sealed class AssemblyWireup : IWireupCommand
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
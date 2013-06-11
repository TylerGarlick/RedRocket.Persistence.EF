using System.Data.Entity;
using FlitBit.IoC;
using FlitBit.Wireup;
using FlitBit.Wireup.Meta;
using RedRocket.Repositories.EF.Tests;

[assembly: WireupDependency(typeof(FlitBit.Dto.AssemblyWireup))]
[assembly: Wireup(typeof(Wireup))]
namespace RedRocket.Repositories.EF.Tests
{
    public class Wireup : IWireupCommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="coordinator">the wireup coordinator</param>
        public void Execute(IWireupCoordinator coordinator)
        {
            Container.Root
                     .ForType<DbContext>()
                     .Register<TestDbContext>()
                     .ResolveAnInstancePerScope()
                     .End();

        }
    }
}

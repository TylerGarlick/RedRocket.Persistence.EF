using FlitBit.IoC;
using FlitBit.IoC.Meta;
using RedRocket.Persistence.EF.ContextFactories;

namespace RedRocket.Persistence.EF
{
    public interface IRepositoryConfigurationSettings
    {
        bool ShouldValidate { get; set; }
        bool ShouldWrapInTransaction { get; set; }
        bool ShouldIncludeDependencies { get; set; }
        bool ShouldTrackEntities { get; set; }
        IDbContextFactory DbContextFactory { get; set; }
    }

    [ContainerRegister(typeof(IRepositoryConfigurationSettings), RegistrationBehaviors.Default, ScopeBehavior = ScopeBehavior.Singleton)]
    public class DefaultRepositoryConfigurationSettings : IRepositoryConfigurationSettings
    {
        public bool ShouldValidate { get; set; }
        public bool ShouldWrapInTransaction { get; set; }
        public bool ShouldIncludeDependencies { get; set; }
        public bool ShouldTrackEntities { get; set; }
        public IDbContextFactory DbContextFactory { get; set; }

        public DefaultRepositoryConfigurationSettings(IDbContextFactory dbContextFactory)
        {
            DbContextFactory = dbContextFactory ?? Create.New<IDbContextFactory>();

            ShouldValidate = true;
            ShouldWrapInTransaction = true;
            ShouldIncludeDependencies = false;
            ShouldTrackEntities = false;
        }
    }
}
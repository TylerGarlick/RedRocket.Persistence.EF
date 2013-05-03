using System.Data.Entity;

namespace RedRocket.Repositories.EntityFramework.Impl.ConnectionStringFactories
{
    public abstract class AbstractInerfaceAwareDbContextFactory : IDbContextFactory
    {
        /// <summary>
        /// Get the connection string based on any set of rules that you determine
        /// </summary>
        /// <param name="entity">Entity that determines the set of rules</param>
        /// <returns>The raw connection string</returns>
        public abstract DbContext GetDbContext<T>(T entity);
    }
}
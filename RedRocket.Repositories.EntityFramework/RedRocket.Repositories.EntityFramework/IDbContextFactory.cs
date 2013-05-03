using System.Data.Entity;

namespace RedRocket.Repositories.EntityFramework
{
    public interface IDbContextFactory
    {
        /// <summary>
        /// Get the connection string based on any set of rules that you determine
        /// </summary>
        /// <param name="entity">Entity that determines the set of rules</param>
        /// <returns>The raw connection string</returns>
        DbContext GetDbContext<T>(T entity);
    }
}
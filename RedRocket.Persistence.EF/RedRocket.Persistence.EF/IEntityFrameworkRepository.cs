using System.Data;
using System.Data.Entity;
using System.Linq;
using RedRocket.Persistence.Common;

namespace RedRocket.Persistence.EF
{
    public interface IEntityFrameworkRepository<T> : IRepository<T> where T : class
    {
        DbContext Context { get; }
        IQueryable<T> Include(string path);
        void ChangeEntityState(T entity, EntityState state);
    }
}
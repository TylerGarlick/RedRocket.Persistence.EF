using System.Data;
using System.Data.Entity;
using System.Linq;

namespace RedRocket.Persistence.Common
{
    public partial interface IRepository<T>
    {
        DbContext Context { get; }
        IQueryable<T> Include(string path);
        void ChangeEntityState(T entity, EntityState state);
    }
}
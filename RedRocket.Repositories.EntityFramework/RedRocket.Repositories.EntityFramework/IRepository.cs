using System.Collections.Generic;
using RedRocket.Repositories.EntityFramework.Impl.EntityFramework;

namespace RedRocket.Repositories.EntityFramework
{
    public interface IRepository<T> : IReadOnlyRepository<T> where T : class
    {
        T Add(T entity);
        T Update(T entity);
        void Delete(T entity);
        
        IEnumerable<ValidationError> Validate(T entity);
    }
}
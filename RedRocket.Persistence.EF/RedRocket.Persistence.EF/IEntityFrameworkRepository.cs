using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using RedRocket.Persistence.Common;
using RedRocket.Utilities.Core.Validation;

namespace RedRocket.Persistence.EF
{
    public partial interface IReadOnlyRepository<T> where T : class
    {
        IQueryable<T> All();
        IQueryable<T> Query(Func<T, bool> predicate);
        T FindWithKey(Expression<Func<T, bool>> predicate);
    }

    public partial interface IRepository<T> : IReadOnlyRepository<T> where T : class
    {
        T Add(T entity);
        T Update(T entity);
        void Delete(T entity);

        DbContext Context { get; }
        IQueryable<T> Include(string path);
        void ChangeEntityState(T entity, EntityState state);

        IEnumerable<ObjectValidationError> Validate(T entity);
    }
}
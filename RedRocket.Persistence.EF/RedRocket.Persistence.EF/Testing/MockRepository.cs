using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using RedRocket.Utilities.Core.Validation;

namespace RedRocket.Persistence.EF.Testing
{
    public class MockRepository<T> : IRepository<T> where T : class
    {
        public List<T> Data { get; set; }

        public MockRepository()
        {
            Data = new List<T>();
        }

        public MockRepository(IEnumerable<T> initialData)
            : this()
        {
            Data.AddRange(initialData);
        }

        public virtual IQueryable<T> All()
        {
            return Data.AsQueryable();
        }

        public virtual IQueryable<T> Query(Func<T, bool> predicate)
        {
            return All().Where(predicate).AsQueryable();
        }

        public virtual T FindWithKey(Expression<Func<T, bool>> predicate)
        {
            return All().FirstOrDefault(predicate);
        }

        public virtual T Add(T entity)
        {
            if (entity.IsObjectValid())
            {
                Data.Add(entity);
                return entity;
            }

            throw new ObjectValidationException(entity.GetValidationErrors());
        }

        public virtual T Update(T entity)
        {
            if (entity.IsObjectValid())
            {
                return entity;
            }
            throw new ObjectValidationException(entity.GetValidationErrors());
        }

        public virtual void Delete(T entity)
        {
            Data.Remove(entity);
        }

        public DbContext Context { get; private set; }
        public IQueryable<T> Include(string path)
        {
            throw new NotImplementedException();
        }

        public void ChangeEntityState(T entity, EntityState state)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectValidationError> Validate(T entity)
        {
            return entity.GetValidationErrors();
        }
    }
}

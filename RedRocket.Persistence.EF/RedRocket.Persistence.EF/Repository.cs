using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using RedRocket.Persistence.EF.ContextFactories;
using RedRocket.Utilities.Core.Validation;

namespace RedRocket.Persistence.EF
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        public DbContext Context { get; private set; }

        public Repository(DbContext dbContext)
        {
            Context = dbContext;
        }

        public Repository(IDbContextFactory dbContextFactory)
        {
            Context = dbContextFactory.GetDbContext(new T());
        }

        public virtual IQueryable<T> All()
        {
            return Context.Set<T>();
        }

        public virtual IQueryable<T> Query(Func<T, bool> predicate)
        {
            return All().Where(predicate).AsQueryable();
        }

        public virtual IQueryable<T> Include(string path)
        {
            return All().Include(path);
        }

        public virtual T FindWithKey(Expression<Func<T, bool>> predicate)
        {
            return All().SingleOrDefault(predicate);
        }

        public virtual T Add(T entity)
        {
            var errors = Validate(entity);
            if (!errors.Any())
            {
                using (var transaction = new TransactionScope())
                {
                    Context.Set<T>().Add(entity);
                    Context.SaveChanges();
                    transaction.Complete();
                    return entity;
                }
            }

            throw new ObjectValidationException(errors);
        }

        public virtual T Update(T entity)
        {
            var errors = Validate(entity);
            if (!errors.Any())
            {
                using (var transaction = new TransactionScope())
                {
                    AttachIfNotAttached(entity);
                    Context.SaveChanges();
                    transaction.Complete();
                    return entity;
                }
            }

            throw new ObjectValidationException(errors);
        }

        public void Delete(T entity)
        {
            using (var transaction = new TransactionScope())
            {
                AttachIfNotAttached(entity);
                ChangeEntityState(entity, EntityState.Deleted);
                Context.SaveChanges();
                transaction.Complete();
            }
        }

        public IEnumerable<ObjectValidationError> Validate(T entity)
        {
            var entityValidationResult = GetValidationErrors(entity);
            return entityValidationResult.ValidationErrors.Select(validationError => new ObjectValidationError()
                                                                                         {
                                                                                             Message = validationError.ErrorMessage,
                                                                                             PropertyName = validationError.PropertyName
                                                                                         });
        }

        public void ChangeEntityState(T entity, EntityState state)
        {
            var entityMeta = Context.Entry(entity);
            entityMeta.State = state;
        }

        void AttachIfNotAttached(T entity)
        {
            var entityMeta = Context.Entry(entity);
            if (entityMeta.State == EntityState.Detached)
            {
                var objectContext = ((IObjectContextAdapter)Context).ObjectContext;
                try
                {
                    var key = objectContext.CreateEntityKey(EntitySetName, entity);
                    var updatedEntity = (T)objectContext.GetObjectByKey(key);
                    Context.Entry(updatedEntity).CurrentValues.SetValues(entity);
                    objectContext.ObjectStateManager.ChangeObjectState(updatedEntity, EntityState.Modified);
                }
                catch
                {
                    Context.Set<T>().Attach(entity);
                }
            }

        }

        DbEntityValidationResult GetValidationErrors(T entity)
        {
            return Context.Entry(entity).GetValidationResult();
        }

        private void CheckIfDifferent(DbEntityEntry entry)
        {
            if (entry.State != EntityState.Modified)
                return;

            if (entry.OriginalValues.PropertyNames.Any(propertyName => !entry.OriginalValues[propertyName].Equals(entry.CurrentValues[propertyName])))
                return;


        }

        string EntitySetName
        {
            get
            {
                return ((IObjectContextAdapter)Context).ObjectContext.CreateObjectSet<T>().EntitySet.Name;
            }
        }
    }
}
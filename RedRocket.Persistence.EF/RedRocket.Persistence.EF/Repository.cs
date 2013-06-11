using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using RedRocket.Persistence.EF.ContextFactories;
using RedRocket.Utilities.Core.Validation;

namespace RedRocket.Persistence.EF
{
    public class Repository<T> : IEntityFrameworkRepository<T> where T : class, new()
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
                    ChangeEntityState(entity, EntityState.Detached);
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
                    entity = Context.Set<T>().Attach(entity);
                    ChangeEntityState(entity, EntityState.Modified);
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
                Context.Set<T>().Attach(entity);
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

        DbEntityValidationResult GetValidationErrors(T entity)
        {
            return Context.Entry(entity).GetValidationResult();
        }
    }
}
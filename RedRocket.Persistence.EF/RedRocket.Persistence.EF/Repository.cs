using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
        public bool ShouldValidate { get; set; }
        public bool ShouldWrapInTransaction { get; set; }

        public Repository(IDbContextFactory dbContextFactory, bool shouldValidate = true, bool shouldWrapInTransaction = true)
        {
            Context = dbContextFactory.GetDbContext(new T());
            ShouldValidate = shouldValidate;
            ShouldWrapInTransaction = shouldWrapInTransaction;
        }

        public virtual IQueryable<T> All()
        {
            return Context.Set<T>();
        }

        public virtual IQueryable<T> Query(Func<T, bool> predicate)
        {
            return All().Where(predicate).AsQueryable();
        }

        public virtual T FindByKey(Expression<Func<T, bool>> predicate)
        {
            return All().SingleOrDefault(predicate);
        }

        public virtual T Add(T entity, bool wrapInTransaction = true)
        {
            if (ShouldValidate)
            {
                var errors = Validate(entity).ToList();
                if (errors.Any())
                    throw new ObjectValidationException(errors);
            }

            if (ShouldWrapInTransaction && wrapInTransaction)
            {
                using (var transaction = new TransactionScope())
                {
                    Context.Set<T>().Add(entity);
                    Context.SaveChanges();
                    transaction.Complete();
                }
            }
            else
            {
                Context.Set<T>().Add(entity);
                Context.SaveChanges();
            }

            return entity;
        }

        public virtual T Update(T entity, bool wrapInTransaction = true)
        {
            if (ShouldValidate)
            {
                var errors = Validate(entity).ToList();
                if (errors.Any())
                    throw new ObjectValidationException(errors);
            }


            if (ShouldWrapInTransaction && wrapInTransaction)
            {
                using (var transaction = new TransactionScope())
                {
                    AttachIfNotAttached(entity);
                    Context.SaveChanges();
                    transaction.Complete();
                }
            }
            else
            {
                AttachIfNotAttached(entity);
                Context.SaveChanges();
            }

            return entity;

        }

        public virtual void Delete(T entity, bool wrapInTransaction = true)
        {
            if (ShouldWrapInTransaction && wrapInTransaction)
            {
                using (var transaction = new TransactionScope())
                {
                    AttachIfNotAttached(entity);
                    ChangeEntityState(entity, EntityState.Deleted);
                    Context.SaveChanges();
                    transaction.Complete();
                }
            }
            else
            {
                AttachIfNotAttached(entity);
                ChangeEntityState(entity, EntityState.Deleted);
                Context.SaveChanges();
            }
        }

        public virtual IEnumerable<ObjectValidationError> Validate(T entity)
        {
            var entityValidationResult = GetValidationErrors(entity);
            return entityValidationResult.ValidationErrors.Select(validationError => new ObjectValidationError
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

        protected void AttachIfNotAttached(T entity)
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

        protected DbEntityValidationResult GetValidationErrors(T entity)
        {
            return Context.Entry(entity).GetValidationResult();
        }

        protected string EntitySetName
        {
            get
            {
                return ((IObjectContextAdapter)Context).ObjectContext.CreateObjectSet<T>().EntitySet.Name;
            }
        }

    }
}
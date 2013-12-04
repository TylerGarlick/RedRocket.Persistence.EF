using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Transactions;
using RedRocket.Persistence.EF.ContextFactories;
using RedRocket.Utilities.Core.Validation;

namespace RedRocket.Persistence.EF
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        readonly Func<DbQuery<T>, DbQuery<T>> _includeMethod;
        readonly HashSet<Type> _visitedTypes;

        public DbContext Context { get; private set; }

        public IRepositoryConfigurationSettings Configuration { get; set; }
        public Repository(IRepositoryConfigurationSettings configuration)
        {
            Configuration = configuration;
            if (Configuration.DbContextFactory != null)
                Context = Configuration.DbContextFactory.GetDbContext(new T());

            _visitedTypes = new HashSet<Type>();
            _includeMethod = d => GetPropsToLoad(typeof(T)).Aggregate(d, (current, prop) => current.Include(prop));
        }

        public virtual IQueryable<T> All()
        {
            var entities = Configuration.ShouldIncludeDependencies ?
                IncludeDependenciesInQuery(Context.Set<T>()) :
                Context.Set<T>();

            return Configuration.ShouldTrackEntities ? entities.AsNoTracking() : entities;
        }

        public virtual IQueryable<T> Query(Func<T, bool> predicate)
        {
            return All().Where(predicate).AsQueryable();
        }

        public virtual T FindByKey(Expression<Func<T, bool>> predicate)
        {
            return All().SingleOrDefault(predicate);
        }

        public virtual T Add(T entity, bool wrapInTransaction = true, bool shouldValidate = true)
        {
            if (Configuration.ShouldValidate && shouldValidate)
            {
                var errors = Validate(entity).ToList();
                if (errors.Any())
                    throw new ObjectValidationException(errors);
            }

            if (Configuration.ShouldWrapInTransaction && wrapInTransaction)
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

        public virtual T Update(T entity, bool wrapInTransaction = true, bool shouldValidate = true)
        {
            if (Configuration.ShouldValidate && shouldValidate)
            {
                var errors = Validate(entity).ToList();
                if (errors.Any())
                    throw new ObjectValidationException(errors);
            }


            if (Configuration.ShouldWrapInTransaction && wrapInTransaction)
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
            if (Configuration.ShouldWrapInTransaction && wrapInTransaction)
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


        IEnumerable<string> GetPropsToLoad(Type type)
        {
            _visitedTypes.Add(type);

            var propsToLoad = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetCustomAttributes(typeof(IncludeAttribute), true).Any());

            foreach (var prop in propsToLoad)
            {
                yield return prop.Name;

                if (_visitedTypes.Contains(prop.PropertyType))
                    continue;

                foreach (var subProp in GetPropsToLoad(prop.PropertyType))
                    yield return prop.Name + "." + subProp;
            }
        }

        DbQuery<T> IncludeDependenciesInQuery(DbSet<T> dbSet)
        {
            return _includeMethod(dbSet);
        }
    }
}
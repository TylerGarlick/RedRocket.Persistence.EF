using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using RedRocket.Utilities.Core.Validation;

namespace RedRocket.Persistence.EF
{
    public interface IReadOnlyRepository<T> where T : class
    {
        /// <summary>
        /// All entities in a given set of T
        /// </summary>
        /// <returns>Queryable Set of T</returns>
        IQueryable<T> All();

        /// <summary>
        /// All entities in a given set of T matching predicate
        /// </summary>
        /// <returns>Queryable Set of T</returns>
        IQueryable<T> Query(Func<T, bool> predicate);


        /// <summary>
        /// Single or Default entity given a predicate
        /// </summary>
        /// <returns>Single or Default Entity of T</returns>
        T FindByKey(Expression<Func<T, bool>> predicate);
    }

    public interface IRepository<T> : IReadOnlyRepository<T> where T : class
    {

        /// <summary>
        /// Performs validation if enabled (ShouldValidate default = true). Adds the entity to the set, and save the set.  The operation is wrapped in a transaction.
        /// </summary>
        /// <param name="entity">The entity to save</param>
        /// <returns>Saved Entity</returns>
        T Add(T entity);

        /// <summary>
        /// Performs validation if enabled (ShouldValidate default = true).  Updates the entity, and saves the entity to the set.  The operation is wrapped in a transaction.
        /// </summary>
        /// <param name="entity">The entity to save</param>
        /// <returns>Saved Entity</returns>
        T Update(T entity);

        /// <summary>
        /// Deletes an entiy from the set, and saves the deletion.  The operation is wrapped in a transaction.
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Currnt Context
        /// </summary>
        DbContext Context { get; }

        /// <summary>
        /// Changes the state of the entity
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="state">The entity state</param>
        void ChangeEntityState(T entity, EntityState state);


        /// <summary>
        /// Performs validation on the entity given the Data Annotations.   Validation is performed on Add and Update.  To
        /// turn off validation, set the ShouldValidate property (default is true).
        /// </summary>
        /// <param name="entity">The Entity</param>
        /// <returns>Set of ObjectValidationErrors</returns>
        IEnumerable<ObjectValidationError> Validate(T entity);
    }
}
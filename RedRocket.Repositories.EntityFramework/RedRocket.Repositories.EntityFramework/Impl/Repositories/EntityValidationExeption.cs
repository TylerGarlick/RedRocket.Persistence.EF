using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using RedRocket.Repositories.EntityFramework.Impl.EntityFramework;

namespace RedRocket.Repositories.EntityFramework.Impl.Repositories
{
    /// <summary>
    /// Exception Helper for DbEntityValidationResult
    /// </summary>
    public class EntityValidationExeption : Exception
    {
        public List<ValidationError> Errors { get; private set; }

        public EntityValidationExeption(DbEntityValidationResult entityValidationResult)
        {
            Errors = new List<ValidationError>();
            foreach (var validationError in entityValidationResult.ValidationErrors)
            {
                Errors.Add(new ValidationError()
                               {
                                   Message = validationError.ErrorMessage,
                                   PropertyName = validationError.PropertyName
                               });
            }
        }
    }
}
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using RedRocket.Persistence.Common.Validation;

namespace RedRocket.Persistence.EF.Validation
{
    /// <summary>
    /// Exception Helper for DbEntityValidationResult
    /// </summary>
    public class EntityValidationExeption : PersistenceValidationException
    {
        public EntityValidationExeption(DbEntityValidationResult entityValidationResult)
            : base(SetupValidationErrorsFromDbEntityValidationResult(entityValidationResult))
        {
        }

        static IEnumerable<PersistenceValidationError> SetupValidationErrorsFromDbEntityValidationResult(DbEntityValidationResult entityValidationResult)
        {
            return entityValidationResult.ValidationErrors.Select(validationError => new PersistenceValidationError()
                                                                                         {
                                                                                             Message = validationError.ErrorMessage,
                                                                                             PropertyName = validationError.PropertyName
                                                                                         });
        }
    }
}
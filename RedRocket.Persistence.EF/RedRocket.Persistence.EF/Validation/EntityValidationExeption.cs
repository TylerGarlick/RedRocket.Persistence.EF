using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using RedRocket.Utilities.Core.Validation;

namespace RedRocket.Persistence.EF.Validation
{
    /// <summary>
    /// Exception Helper for DbEntityValidationResult
    /// </summary>
    public class EntityValidationExeption : ObjectValidationException
    {
        public EntityValidationExeption(DbEntityValidationResult entityValidationResult)
            : base(SetupValidationErrorsFromDbEntityValidationResult(entityValidationResult))
        {
        }

        static IEnumerable<ObjectValidationError> SetupValidationErrorsFromDbEntityValidationResult(DbEntityValidationResult entityValidationResult)
        {
            return entityValidationResult.ValidationErrors.Select(validationError => new ObjectValidationError()
                                                                                         {
                                                                                             Message = validationError.ErrorMessage,
                                                                                             PropertyName = validationError.PropertyName
                                                                                         });
        }
    }
}
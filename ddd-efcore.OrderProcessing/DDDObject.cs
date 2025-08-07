using System.ComponentModel.DataAnnotations;

namespace ddd_efcore.OrderProcessing
{
    public class DDDObject
    {
        internal void Validate()
        {
            var className = GetType().Name;

            var ctx = new ValidationContext(this);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(this, ctx, results, true);

            if (!isValid)
            {
                var errors = string.Join(", ", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"{className} validation failed: {errors}");
            }

            this.ValidateSpecific();
        }

        protected virtual void ValidateSpecific()
        {
            // Override in derived classes for specific validation logic
        }
    }
}

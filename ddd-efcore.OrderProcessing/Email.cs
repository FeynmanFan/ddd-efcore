using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ddd_efcore.OrderProcessing
{
    // Value Object: Represents an email address (immutable, no identity)
    public class Email: DDDObject
    {
        private readonly string _value;

        // Parameterless constructor for EF Core
        protected Email()
        {
            this.Strategy = ComparisonStrategy.Aristotelian;
        }

        // Constructor with validation
        private Email(string value)
        {
            this.Strategy = ComparisonStrategy.Aristotelian;

            _value = value;

            this.Validate();
        }

        public static Email Create(string value)
        {
            return new Email(value);
        }

        [Required(ErrorMessage = "Email address cannot be empty!")]
        [StringLength(254, MinimumLength = 3, ErrorMessage = "Email address must be between 3 and 254 characters!")]
        [EmailFormat(ErrorMessage = "Invalid email format!")]
        public string Value => _value;

        protected override object[] GetComparisonValues()
        {
            return [this.Value];
        }
    }

    public class EmailFormatAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Email address cannot be null");
            }

            string email = value.ToString()!;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(email, pattern))
            {
                return new ValidationResult("Invalid email format");
            }

            return ValidationResult.Success;
        }
    }
}
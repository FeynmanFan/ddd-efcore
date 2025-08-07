using System;

namespace ddd_efcore.OrderProcessing
{
    // Value Object: Represents an email address (immutable, no identity)
    public class Email
    {
        private readonly string _value;

        // Parameterless constructor for EF Core
        private Email()
        {
            _value = null!; // Suppress nullability warning; EF Core sets the value
        }

        // Constructor with validation
        private Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty!", nameof(value));

            if (!value.Contains("@"))
                throw new ArgumentException("Email must contain '@' character!", nameof(value));

            _value = value;
        }

        public static Email Create(string value)
        {
            return new Email(value);
        }

        public string Value => _value;
    }
}
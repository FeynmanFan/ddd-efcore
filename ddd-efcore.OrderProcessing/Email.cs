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
        public Email(string value)
        {
            _value = value;
        }

        public string Value => _value;
    }
}
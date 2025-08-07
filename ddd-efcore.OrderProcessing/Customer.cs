using System.ComponentModel.DataAnnotations;

namespace ddd_efcore.OrderProcessing
{
    // Entity: Customer with identity and lifecycle
    public class Customer
    {
        // Private constructor for EF Core
        private Customer()
        {
        }

        // Private constructor for creating a customer
        private Customer(Guid id, string name, Email email)
        {
            Id = id != Guid.Empty ? id : throw new ArgumentException("Customer ID cannot be empty!");
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Name cannot be empty!");
            Email = email ?? throw new ArgumentNullException(nameof(email));
            CreatedAt = DateTime.UtcNow;
        }

        private Customer(string name, Email email)
            : this(Guid.NewGuid(), name, email)
        {
        }

        public static Customer Create(string name, Email email)
        {
            return new Customer(name, email);
        }

        [Key]
        public Guid Id { get; private set; } // Unique ID for the customer
        public string Name { get; private set; } // Customer's name
        public Email Email { get; private set; } // Customer's email (value object)
        public DateTime CreatedAt { get; private set; } // When the customer was created

        // Domain behavior: Update customer name
        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Name cannot be empty!");
            Name = newName;
        }

        // Domain behavior: Update email
        public void UpdateEmail(Email newEmail)
        {
            Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
        }
    }
}
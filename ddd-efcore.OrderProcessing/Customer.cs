using System.ComponentModel.DataAnnotations;

namespace ddd_efcore.OrderProcessing
{
    // Entity: Customer with identity and lifecycle
    public class Customer : DDDObject
    {
        // Private constructor for EF Core
        private Customer()
        {
            this.Strategy = ComparisonStrategy.Platonic;
        }
            
        // Private constructor for creating a customer
        private Customer(Guid id, string name, Email email)
        {
            this.Strategy = ComparisonStrategy.Platonic; 

            Id = id;
            Name = name;
            Email = email;
            CreatedAt = DateTime.UtcNow;

            this.Validate();
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

        [Required(ErrorMessage="Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; private set; } // Customer's name

        [Required(ErrorMessage="Email is required.")]
        public Email Email { get; private set; } // Customer's email (value object)
        public DateTime CreatedAt { get; private set; } // When the customer was created

        // Domain behavior: Update customer name
        public void UpdateName(string newName)
        {
            Name = newName;

            this.Validate();
        }

        // Domain behavior: Update email
        public void UpdateEmail(Email newEmail)
        {
            Email = newEmail;

            this.Validate();
        }

        protected override void ValidateSpecific()
        {
            if (Id == Guid.Empty)
            {
                throw new InvalidOperationException($"Customer ID must be set before validation.");
            }
        }

        protected override object[] GetComparisonValues()
        {
            return [this.Id];
        }
    }
}
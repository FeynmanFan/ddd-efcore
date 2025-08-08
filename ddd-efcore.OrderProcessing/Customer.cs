namespace ddd_efcore.OrderProcessing
{
    using System.ComponentModel.DataAnnotations;

    // Entity: Customer with identity and lifecycle
    public class Customer : DDDEntity
    {
        private readonly List<Order> _orders = [];

        // Private constructor for EF Core
        private Customer()
        {
            this.Strategy = ComparisonStrategy.Platonic;
        }
            
        // Private constructor for creating a customer
        private Customer(Guid id, string name, Email email):base(id)
        {
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

        [Required(ErrorMessage="Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; private set; } // Customer's name

        [Required(ErrorMessage="Email is required.")]
        public Email Email { get; private set; } // Customer's email (value object)
        public DateTime CreatedAt { get; private set; } // When the customer was created

        public IReadOnlyList<Order> Orders => _orders.AsReadOnly();

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

        public void PlaceOrder(decimal amount, DateTime orderDate)
        {
            var order = new Order(amount, orderDate, this.Id);
            _orders.Add(order);
        }

        public void ConfirmOrder(Guid orderId)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
                throw new OrderValidityException("Order not found.");
            order.Confirm();
        }

        public void CancelOrder(Guid orderId)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
                throw new OrderValidityException("Order not found.");
            order.Cancel();
        }
    }
}
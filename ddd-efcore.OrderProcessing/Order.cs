namespace ddd_efcore.OrderProcessing
{
    public class Order: DDDEntity
    {
        public virtual Payment? Payment { get; private set; }

        protected Order() { } // Private constructor for EF Core

        internal Order(decimal amount, DateTime orderDate, Guid customerId):base(Guid.NewGuid())
        {
            Amount = amount;
            OrderDate = GuardAgainstInvalidDate(orderDate);
            Status = OrderStatus.Pending;
            CustomerId = customerId;
        }

        public decimal Amount { get; private set; }
        public DateTime OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public Guid CustomerId { get; private set; } // Foreign key

        // Domain logic
        public void Confirm()
        {
            if (Status != OrderStatus.Pending)
                throw new OrderValidityException("Only pending orders can be confirmed.");
            Status = OrderStatus.Confirmed;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Cancelled)
                throw new OrderValidityException("Order is already cancelled.");
            Status = OrderStatus.Cancelled;
        }

        private static DateTime GuardAgainstInvalidDate(DateTime orderDate)
        {
            if (orderDate > DateTime.UtcNow)
                throw new OrderValidityException("Order date cannot be in the future.");
            return orderDate;
        }

        public void Pay()
        {
            this.Payment = Payment.Create(this.Amount, DateTime.UtcNow, this.Id);
        }
    }

    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }

    public class OrderValidityException : ApplicationException
    {
        public OrderValidityException(string message) : base(message) { }
    }
}
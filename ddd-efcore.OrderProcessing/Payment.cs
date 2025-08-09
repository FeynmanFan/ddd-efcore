namespace ddd_efcore.OrderProcessing
{
    public class Payment:DDDEntity
    {
        protected Payment() { } // Private constructor for EF Core

        protected Payment(decimal amount, DateTime paymentDate, Guid orderId) : base(Guid.NewGuid())
        {
            Amount = amount;
            PaidOn = paymentDate;
            OrderId = orderId;
        }

        public decimal Amount { get; private set; } // Payment amount
        public DateTime PaidOn { get; private set; } // When the payment was made
        public Guid OrderId { get; private set; } // Foreign key to Order
        
        public static Payment Create(decimal amount, DateTime paymentDate, Guid orderId)
        {
            return new Payment(amount, paymentDate, orderId);
        }
    }
}
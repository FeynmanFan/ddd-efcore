namespace ddd_efcore.OrderProcessing
{
    using MediatR;

    public interface IDomainEvent
    {
        Guid EventId { get; }
        DateTime OccurredOn { get; }
    }

    public class CustomerUpdatedEvent : IDomainEvent, INotification
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public Guid CustomerId { get; }
        public string Name { get; }
        public string Email { get; }

        public CustomerUpdatedEvent(Guid customerId, string name, string email) : this(Guid.NewGuid(), DateTime.UtcNow, customerId, name, email)
        {
        }

        public CustomerUpdatedEvent(Guid eventId, DateTime occurredOn, Guid customerId, string name, string email)
        {
            EventId = eventId;
            OccurredOn = occurredOn;
            CustomerId = customerId;
            Name = name;
            Email = email;

        }
    }
}
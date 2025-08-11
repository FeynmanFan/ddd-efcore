using MediatR;

namespace ddd_efcore.OrderProcessing
{
    public class CustomerUpdatedEventHandler : INotificationHandler<CustomerUpdatedEvent>
    {
        public Task Handle(CustomerUpdatedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Customer Updated Event: " +
                $"CustomerId={notification.CustomerId}, " +
                $"Name={notification.Name}, " +
                $"Email={notification.Email}, " +
                $"OccurredOn={notification.OccurredOn:yyyy-MM-dd HH:mm:ss}");
            return Task.CompletedTask;
        }
    }
}
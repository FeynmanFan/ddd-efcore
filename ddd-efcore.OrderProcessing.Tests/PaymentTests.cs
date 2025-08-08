using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ddd_efcore.OrderProcessing.Tests
{
    public class PaymentTests
    {
        [Fact]
        public void Payment_Creation_ShouldSucceed()
        {
            var customer = Customer.Create("John Doe", Email.Create("test@test.com"));

            customer.PlaceOrder(100.00m, DateTime.UtcNow);

            var order = customer.Orders.Single<Order>();

            order.Pay();

            var payment = order.Payment;

            Assert.Equal(100.00m, payment.Amount);
        }
    }
}

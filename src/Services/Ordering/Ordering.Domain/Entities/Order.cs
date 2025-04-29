using Ordering.Domain.Common;

namespace Ordering.Domain.Entities
{
    public class Order : EntityBase
    {
        public string UserName { get; set; }

        public decimal TotalPrice { get; set; }

        // BillingAddress
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string AddressLine { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        // Payment
        public string CardName { get; set; }

        public string CardNumber { get; set; }

        public string Expiration { get; set; }

        public string CVV { get; set; }

        public int PaymentMethod { get; set; }

        // Constructor to ensure LastModifiedBy and LastModifiedDate are set
        public Order()
        {
            // Set default values to prevent null errors
            CreatedBy = "Seeder"; // or some default value
            CreatedDate = DateTime.UtcNow;
            LastModifiedBy = "Seeder"; // or some default value
            LastModifiedDate = DateTime.UtcNow;
        }
    }
}

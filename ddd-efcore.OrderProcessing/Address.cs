namespace ddd_efcore.OrderProcessing
{
    public class Address : DDDObject
    {
        protected Address()
        {
            this.Strategy = ComparisonStrategy.Platonic;
        }

        private Address(string street, string zipCode)
        {
            Street = street;
            ZipCode = zipCode;
        }

        public string Street { get; }
        public string ZipCode { get; }

        public static Address Create(string street, string cityName, string state, string zipCode, ICityValidator validator)
        {
            var validationErrors = new List<string>();

            // Validate Street
            if (string.IsNullOrWhiteSpace(street))
                validationErrors.Add("Street is required.");

            // Validate City, State, ZipCode
            if (!validator.IsValidCity(zipCode, cityName, state))
                validationErrors.Add($"Invalid address: Zip code {zipCode} does not match city {cityName} or state {state}.");

            // Throw if any validation errors
            if (validationErrors.Any())
                throw new AddressValidityException($"Invalid address: {string.Join("; ", validationErrors)}");

            return new Address(street, zipCode);
        }

        protected override object[] GetComparisonValues()
        {
            return [Street, ZipCode];
        }
    }

    public class AddressValidityException : ApplicationException
    {
        public AddressValidityException(string message) : base(message) { }
    }
}
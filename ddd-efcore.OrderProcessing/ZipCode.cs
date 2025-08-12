namespace ddd_efcore.OrderProcessing
{
    public class ZipCode : DDDObject
    {
        protected ZipCode() { }

        public ZipCode(string zipCode, string city, string state)
        {
            ZipCodeValue = zipCode;
            City = city;
            State = state;
        }

        public string ZipCodeValue { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }

        protected override object[] GetComparisonValues() => [ZipCodeValue, City, State];
    }
}
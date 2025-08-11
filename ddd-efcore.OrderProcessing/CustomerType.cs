namespace ddd_efcore.OrderProcessing
{
    public enum CustomerTypeEnum
    {
        Private = 1,
        Business = 2
    }

    public class CustomerType : DDDObject
    {
        private CustomerType(CustomerTypeEnum value)
        {
            Value = value;
            this.Strategy = ComparisonStrategy.Platonic;
        }

        public CustomerTypeEnum Value { get; }

        public static CustomerType Create(CustomerTypeEnum value)
        {
            return new CustomerType(value);
        }

        public static CustomerType Create(int value)
        {
            if (!Enum.IsDefined(typeof(CustomerTypeEnum), value))
                throw new ArgumentException($"Invalid customer type value: {value}. Allowed values are 1 (Private) or 2 (Business).");

            return new CustomerType((CustomerTypeEnum)value);
        }

        protected override object[] GetComparisonValues()
        {
            return [Value];
        }
    }
}
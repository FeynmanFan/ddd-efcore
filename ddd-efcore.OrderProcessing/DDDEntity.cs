namespace ddd_efcore.OrderProcessing
{
    public abstract class DDDEntity : DDDObject
    {
        protected DDDEntity()
        {
        }

        protected DDDEntity(Guid id)
        {
            Id = id;
            this.Strategy = ComparisonStrategy.Platonic; // Use Platonic strategy for identity
        }

        public Guid Id { get; protected set; }

        protected override object[] GetComparisonValues()
        {
            return [Id];
        }


        protected override void ValidateSpecific()
        {
            if (Id == Guid.Empty)
            {
                throw new InvalidOperationException($"Customer ID must be set before validation.");
            }
        }
    }
}

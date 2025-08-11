using System.ComponentModel.DataAnnotations;

namespace ddd_efcore.OrderProcessing
{
    public enum ComparisonStrategy
    {
        Platonic = 0, 
        Aristotelian = 1
    }

    public abstract class DDDObject
    {
        protected ComparisonStrategy Strategy { get; set; }
        private readonly List<IDomainEvent> _domainEvents = [];

        protected abstract object[] GetComparisonValues();

        internal void Validate()
        {
            var className = GetType().Name;

            var ctx = new ValidationContext(this);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(this, ctx, results, true);

            if (!isValid)
            {
                var errors = string.Join(", ", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"{className} validation failed: {errors}");
            }

            this.ValidateSpecific();
        }

        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            this._domainEvents.Clear();
        }

        protected virtual void ValidateSpecific()
        {
            // Override in derived classes for specific validation logic
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null || GetType() != obj.GetType()) return false;

            var other = (DDDObject)obj;
            var thisValues = GetComparisonValues();
            var otherValues = other.GetComparisonValues();

            if (thisValues.Length != otherValues.Length) return false;

            for (int i = 0; i < thisValues.Length; i++)
            {
                if (!Equals(thisValues[i], otherValues[i])) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            if (Strategy == ComparisonStrategy.Platonic)
            {
                return (int)GetComparisonValues().First(); // Directly use Id for entities
            }
            else // Aristotelian
            {
                var values = GetComparisonValues();
                unchecked
                {
                    int hash = 17;
                    foreach (var value in values)
                    {
                        hash = hash * 23 + (value?.GetHashCode() ?? 0);
                    }
                    return hash;
                }
            }
        }

        public static bool operator ==(DDDObject? left, DDDObject? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;

            return left.Equals(right);
        }

        public static bool operator !=(DDDObject? left, DDDObject? right)
        {
            return !(left == right);
        }
    }
}

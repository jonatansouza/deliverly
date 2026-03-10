namespace DeliverlyCore.Shared.Domain
{
    public abstract class ValueObject<T> where T : ValueObject<T>
    {
        public abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj is not T other) return false;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode() =>
            GetEqualityComponents()
                .Aggregate(1, (hash, component) => HashCode.Combine(hash, component));

        public static bool operator ==(ValueObject<T> left, ValueObject<T> right) => left.Equals(right);
        public static bool operator !=(ValueObject<T> left, ValueObject<T> right) => !left.Equals(right);
    }
}

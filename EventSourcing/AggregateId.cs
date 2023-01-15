namespace EventSourcing;

public abstract class AggregateId<T> : Value<AggregateId<T>> where T : AggregateRoot
{
    protected AggregateId(long value)
    {
        if (value == default)
            throw new ArgumentNullException(
                nameof(value), 
                "The Id cannot be empty");
            
        Value = value;
    }

    public long Value { get; }
        
    public static implicit operator long(AggregateId<T> self) => self.Value;

    public override string ToString() => Value.ToString();
}
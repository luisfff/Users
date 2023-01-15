using EventSourcing;

namespace Users.Domain;

public class DisplayName : Value<DisplayName>
{
    internal DisplayName(string displayName) => Value = displayName;

    // Satisfy the serialization requirements
    protected DisplayName() { }
    public string Value { get; }

    public static DisplayName FromString(
        string displayName
    )
    {
        if (displayName.IsEmpty())
            throw new ArgumentNullException(nameof(FullName));

        return new(displayName);
    }

    public static implicit operator string(DisplayName displayName)
        => displayName.Value;
}
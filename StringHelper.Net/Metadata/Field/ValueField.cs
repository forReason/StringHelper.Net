using StringHelper.Net.Metadata.Token;
using StringHelper.Net.Metadata.Token.Derivates;

namespace StringHelper.Net.Metadata.StringField;

public sealed class ValueField : IEquatable<ValueField>
{
    public FieldName FieldName { get; }
    public double Value { get; }

    public ValueField(FieldName fieldName, double value)
    {
        FieldName = fieldName;
        Value = value;
    }

    public bool Equals(ValueField other) => FieldName.Equals(other.FieldName) && Value == other.Value;

    public override bool Equals(object obj) => obj is ValueField other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            return (FieldName.GetHashCode() * 397) ^ Value.GetHashCode();
        }
    }
}
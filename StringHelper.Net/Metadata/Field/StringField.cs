using StringHelper.Net.Metadata.Token;
using StringHelper.Net.Metadata.Token.Derivates;

namespace StringHelper.Net.Metadata.StringField;

public sealed class StringField : IEquatable<StringField>
{
    public FieldName FieldName { get; }
    public FieldValue Value { get; }

    public StringField(FieldName fieldName, FieldValue value)
    {
        FieldName = fieldName;
        Value = value;
    }

    public bool Equals(StringField other) => FieldName.Equals(other.FieldName) && Value.Equals(other.Value);

    public override bool Equals(object obj) => obj is StringField other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            return (FieldName.GetHashCode() * 397) ^ Value.GetHashCode();
        }
    }
}
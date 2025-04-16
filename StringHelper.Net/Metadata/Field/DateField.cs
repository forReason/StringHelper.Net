using StringHelper.Net.Metadata.Token;
using StringHelper.Net.Metadata.Token.Derivates;

namespace StringHelper.Net.Metadata.StringField;

public sealed class DateField : IEquatable<DateField>
{
    public FieldName FieldName { get; }
    public DateTime Date { get; }

    public DateField(FieldName fieldName, DateTime date)
    {
        FieldName = fieldName;
        Date = date;
    }

    public bool Equals(DateField other) => FieldName.Equals(other.FieldName) && Date.Equals(other.Date);

    public override bool Equals(object obj) => obj is DateField other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            return (FieldName.GetHashCode() * 397) ^ Date.GetHashCode();
        }
    }
}
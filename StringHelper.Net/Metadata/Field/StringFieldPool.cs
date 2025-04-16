using System.Collections.Concurrent;
using StringHelper.Net.Metadata.Token;
using StringHelper.Net.Metadata.Token.Derivates;

namespace StringHelper.Net.Metadata.StringField;

public class StringFieldPool
{
    public NamedTokenPool<FieldName> FieldNames = new ((id, name) => new FieldName(id, name));
    public NamedTokenPool<FieldValue> FieldValues = new ((id, name) => new FieldValue(id, name));
    public StringField GetStringField(string fieldName, string fieldValue)
    {
        FieldName name = FieldNames.GetOrCreateToken(fieldName);
        FieldValue value = FieldValues.GetOrCreateToken(fieldValue);
        if (!AllFields.TryGetValue((name,value), out StringField? field))
        {
            AllFields[(name,value)] = field = new StringField(name, value);
        }
        return field;
    }
    public ConcurrentDictionary<(FieldName, FieldValue), StringField> AllFields
        = new ConcurrentDictionary<(FieldName, FieldValue), StringField>();
}
using System.Collections.Concurrent;
using StringHelper.Net.Metadata.Token;
using StringHelper.Net.Metadata.Token.Derivates;

namespace StringHelper.Net.Metadata.StringField;

public class StringFieldPool
{
    public NamedTokenPool<FieldName> FieldNames = new ((id, name) => new FieldName(id, name));
    public NamedTokenPool<FieldValue> FieldValues = new ((id, name) => new FieldValue(id, name));

    public ConcurrentDictionary<(FieldName, FieldValue), StringField> AllFields
        = new ConcurrentDictionary<(FieldName, FieldValue), StringField>();
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
    public StringField InsertStringField(StringField field)
    {
        if (!AllFields.TryGetValue((field.FieldName,field.Value), out StringField? existingField))
        {
            AllFields[(field.FieldName,field.Value)] = existingField = new StringField(field.FieldName,field.Value);
        }
        return existingField;
    }
    public List<StringField> InsertStringFields(IEnumerable<StringField> field)
    {
        List<StringField> fields = new List<StringField>();
        foreach (var f in field)
        {
            fields.Add(InsertStringField(f));
        }
        return fields;
    }
}
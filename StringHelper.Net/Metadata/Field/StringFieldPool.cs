using System.Collections.Concurrent;
using StringHelper.Net.Metadata.Token;
using StringHelper.Net.Metadata.Token.Derivates;

namespace StringHelper.Net.Metadata.StringField;

public class StringFieldPool
{
    public NamedTokenPool<FieldName> FieldNames = new((id, name) => new FieldName(id, name));
    public NamedTokenPool<FieldValue> FieldValues = new((id, name) => new FieldValue(id, name));

    public ConcurrentDictionary<(FieldName, FieldValue), StringField> AllFields
        = new ConcurrentDictionary<(FieldName, FieldValue), StringField>();

    private readonly ConcurrentDictionary<FieldName, int> _fieldNameRefCount = new();
    private readonly ConcurrentDictionary<FieldValue, int> _fieldValueRefCount = new();

    public StringField GetOrCreateStringField(string fieldName, string fieldValue)
    {
        FieldName name = FieldNames.GetOrCreateToken(fieldName);
        FieldValue value = FieldValues.GetOrCreateToken(fieldValue);

        if (!AllFields.TryGetValue((name, value), out StringField? field))
        {
            field = new StringField(name, value);
            AllFields[(name, value)] = field;

            // New field -> increase reference counts
            _fieldNameRefCount.AddOrUpdate(name, 1, (_, count) => count + 1);
            _fieldValueRefCount.AddOrUpdate(value, 1, (_, count) => count + 1);
        }

        return field;
    }
    public List<StringField> GetOrCreateStringFields(IEnumerable<(string fieldName, string fieldValue)> fields)
    {
        List<StringField> insertedFields = new();
        foreach (var f in fields)
        {
            GetOrCreateStringField(f.fieldName, f.fieldValue);
        }
        return insertedFields;
    }

    public StringField InsertStringField(StringField field)
    {
        if (!AllFields.TryGetValue((field.FieldName, field.Value), out StringField? existingField))
        {
            existingField = new StringField(field.FieldName, field.Value);
            AllFields[(field.FieldName, field.Value)] = existingField;

            // New field -> increase reference counts
            _fieldNameRefCount.AddOrUpdate(field.FieldName, 1, (_, count) => count + 1);
            _fieldValueRefCount.AddOrUpdate(field.Value, 1, (_, count) => count + 1);
        }

        return existingField;
    }

    public List<StringField> InsertStringFields(IEnumerable<StringField> fields)
    {
        List<StringField> insertedFields = new();
        foreach (var f in fields)
        {
            insertedFields.Add(InsertStringField(f));
        }
        return insertedFields;
    }

    public void RemoveStringField(StringField field)
    {
        bool success = AllFields.TryRemove((field.FieldName, field.Value), out _);
        if (!success)
            return;

        // Decrease reference counts
        if (_fieldNameRefCount.AddOrUpdate(field.FieldName, 0, (_, count) => count - 1) <= 0)
        {
            _fieldNameRefCount.TryRemove(field.FieldName, out _);
            FieldNames.RemoveToken(field.FieldName);
        }

        if (_fieldValueRefCount.AddOrUpdate(field.Value, 0, (_, count) => count - 1) <= 0)
        {
            _fieldValueRefCount.TryRemove(field.Value, out _);
            FieldValues.RemoveToken(field.Value);
        }
    }
}

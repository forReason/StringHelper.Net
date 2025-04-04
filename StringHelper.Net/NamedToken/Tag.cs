namespace StringHelper.Net.NamedToken;

public sealed class Tag : INamedToken
{
    public Guid Id { get; }
    public string Name { get; }

    private readonly int _HashCode;

    public Tag(Guid id, string name)
    {
        Id = id;
        Name = name;
        _HashCode = id.GetHashCode();
    }

    public override bool Equals(object? obj) => Equals(obj as Tag);

    public bool Equals(INamedToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return Id == other.Id;
    }

    public override int GetHashCode() => _HashCode;

    public static bool operator ==(Tag? left, Tag? right) => Equals(left, right);
    public static bool operator !=(Tag? left, Tag? right) => !Equals(left, right);

    public int CompareTo(INamedToken? other)
    {
        if (other is null) return 1;
        return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }
}
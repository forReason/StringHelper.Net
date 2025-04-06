namespace StringHelper.Net.NamedTokenNs;

/// <summary>
/// Represents a uniquely identified tag with a globally unique <see cref="Guid"/> and a string <see cref="Name"/>.
/// Tags are compared based on their <see cref="Id"/> and sorted by their <see cref="Name"/> (case-insensitive).
/// </summary>
/// <remarks>
/// This class is typically used in conjunction with <see cref="NamedTokenPool{T}"/> to ensure deduplicated usage of tags
/// </remarks>
public sealed class Tag : INamedToken
{
    /// <summary>
    /// Gets the globally unique identifier for this tag.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the human-readable name of the tag.
    /// This value is preserved as provided (including casing), but comparison is case-insensitive.
    /// </summary>
    public string Name { get; }

    private readonly int _HashCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tag"/> class.
    /// </summary>
    /// <param name="id">The globally unique identifier for the tag.</param>
    /// <param name="name">The name of the tag. It should already be trimmed and validated externally.</param>
    public Tag(Guid id, string name)
    {
        Id = id;
        Name = name;
        _HashCode = id.GetHashCode();
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current tag based on ID.
    /// </summary>
    /// <param name="obj">The object to compare with the current tag.</param>
    /// <returns><c>true</c> if the specified object is a tag with the same ID; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => Equals(obj as Tag);

    /// <summary>
    /// Determines whether the specified <see cref="INamedToken"/> is equal to the current tag based on ID.
    /// </summary>
    /// <param name="other">The tag to compare with the current tag.</param>
    /// <returns><c>true</c> if the specified tag has the same ID; otherwise, <c>false</c>.</returns>
    public bool Equals(INamedToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return Id == other.Id;
    }

    /// <summary>
    /// Returns the hash code for this tag, based on its ID.
    /// </summary>
    /// <returns>A hash code for the current tag.</returns>
    public override int GetHashCode() => _HashCode;

    /// <summary>
    /// Determines whether two tags are equal by comparing their IDs.
    /// </summary>
    /// <param name="left">The first tag to compare.</param>
    /// <param name="right">The second tag to compare.</param>
    /// <returns><c>true</c> if both tags have the same ID or are the same instance; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Tag? left, Tag? right) => Equals(left, right);

    /// <summary>
    /// Determines whether two tags are not equal by comparing their IDs.
    /// </summary>
    /// <param name="left">The first tag to compare.</param>
    /// <param name="right">The second tag to compare.</param>
    /// <returns><c>true</c> if the tags have different IDs; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Tag? left, Tag? right) => !Equals(left, right);

    /// <summary>
    /// Compares the current tag with another tag by name, ignoring case.
    /// </summary>
    /// <param name="other">The tag to compare to.</param>
    /// <returns>
    /// A value less than zero if this instance precedes <paramref name="other"/> in the sort order;
    /// zero if they are equal; greater than zero if it follows <paramref name="other"/>.
    /// </returns>
    public int CompareTo(INamedToken? other)
    {
        if (other is null) return 1;
        return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }
}
namespace StringHelper.Net.NamedTokenNs;

/// <summary>
/// Represents a uniquely identified named token with a globally unique <see cref="Guid"/> and a string <see cref="Name"/>.
/// Tokens are compared based on their <see cref="Id"/> and sorted by their <see cref="Name"/> (case-insensitive).
/// </summary>
/// <remarks>
/// This class is typically used in conjunction with <see cref="NamedTokenPool{T}"/> to ensure deduplicated usage of named values,
/// such as tags, metadata keys, or similar domain-specific identifiers.
/// </remarks>
public sealed class NamedToken : INamedToken
{
    /// <summary>
    /// Gets the globally unique identifier for this token.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets the human-readable name of the token.
    /// This value is preserved as provided (including casing), but comparison is case-insensitive.
    /// </summary>
    public string Name { get; }

    private readonly int _HashCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedToken"/> class.
    /// </summary>
    /// <param name="id">The globally unique identifier for the token.</param>
    /// <param name="name">The name of the token. It should already be trimmed and validated externally.</param>
    public NamedToken(Guid id, string name)
    {
        this.Id = id;
        this.Name = name;
        _HashCode = id.GetHashCode();
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current token based on ID.
    /// </summary>
    /// <param name="obj">The object to compare with the current token.</param>
    /// <returns><c>true</c> if the specified object is a token with the same ID; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => Equals(obj as Tag);

    /// <summary>
    /// Determines whether the specified <see cref="INamedToken"/> is equal to the current token based on ID.
    /// </summary>
    /// <param name="other">The token to compare with the current token.</param>
    /// <returns><c>true</c> if the specified token has the same ID; otherwise, <c>false</c>.</returns>
    public bool Equals(INamedToken? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return this.Id == other.Id;
    }

    /// <summary>
    /// Returns the hash code for this token, based on its ID.
    /// </summary>
    /// <returns>A hash code for the current token.</returns>
    public override int GetHashCode() => _HashCode;

    /// <summary>
    /// Determines whether two tokens are equal by comparing their IDs.
    /// </summary>
    /// <param name="left">The first token to compare.</param>
    /// <param name="right">The second token to compare.</param>
    /// <returns><c>true</c> if both tokens have the same ID or are the same instance; otherwise, <c>false</c>.</returns>
    public static bool operator ==(NamedToken? left, NamedToken? right) => Equals(left, right);

    /// <summary>
    /// Determines whether two tokens are not equal by comparing their IDs.
    /// </summary>
    /// <param name="left">The first token to compare.</param>
    /// <param name="right">The second token to compare.</param>
    /// <returns><c>true</c> if the tokens have different IDs; otherwise, <c>false</c>.</returns>
    public static bool operator !=(NamedToken? left, NamedToken? right) => !Equals(left, right);

    /// <summary>
    /// Compares the current token with another token by name, ignoring case.
    /// </summary>
    /// <param name="other">The token to compare to.</param>
    /// <returns>
    /// A value less than zero if this instance precedes <paramref name="other"/> in the sort order;
    /// zero if they are equal; greater than zero if it follows <paramref name="other"/>.
    /// </returns>
    public int CompareTo(INamedToken? other)
    {
        if (other is null) return 1;
        return string.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }
}
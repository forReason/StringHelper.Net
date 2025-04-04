using System.Collections.Concurrent;
using Multithreading_Library.ThreadControl;

namespace StringHelper.Net.NamedTokenNs;

/// <summary>
/// A thread-safe pool for managing and deduplicating named tokens of type <typeparamref name="T"/>.
/// Ensures that tokens with the same normalized name are reused across the system.
/// </summary>
/// <typeparam name="T">
/// A class implementing <see cref="INamedToken"/>, which provides a unique <see cref="Guid"/> Id and a string <see cref="INamedToken.Name"/>.
/// </typeparam>
/// <remarks>
/// To use this pool, pass a factory function that constructs a new instance of your token type from a <see cref="Guid"/> and a normalized <see cref="string"/> name.
///
/// <para>Example usage with a custom token type <c>Tag</c>:</para>
/// <code>
/// var tagPool = new NamedTokenPool&lt;Tag&gt;((id, name) =&gt; new Tag(id, name));
/// var tag = tagPool.GetOrCreateToken("Important");
/// </code>
/// </remarks>
public class NamedTokenPool<T> where T : class, INamedToken
{
    private readonly Func<Guid, string, T> _factory;
    private readonly IDReaderWriterLocks<string> _NameLocks = new();
    private readonly IDReaderWriterLocks<Guid> _GuidLocks = new();
    private readonly ConcurrentDictionary<string, T> _NameLookup =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<Guid, T> _GuidLookup = new();

    /// <summary>
    /// Gets the number of tokens currently stored in the pool.
    /// </summary>
    public int Count => _NameLookup.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedTokenPool{T}"/> class.
    /// </summary>
    /// <remarks>
    /// To use this pool, pass a factory function that constructs a new instance of your token type from a <see cref="Guid"/> and a normalized <see cref="string"/> name.
    ///
    /// <para>Example usage with a custom token type <c>Tag</c>:</para>
    /// <code>
    /// var tagPool = new NamedTokenPool&lt;Tag&gt;((id, name) =&gt; new Tag(id, name));
    /// var tag = tagPool.GetOrCreateToken("Important");
    /// </code>
    /// </remarks>
    /// <param name="factory">A delegate that creates a new instance of <typeparamref name="T"/> given a <see cref="Guid"/> and a normalized <see cref="string"/> name.</param>
    public NamedTokenPool(Func<Guid, string, T> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Gets all tokens currently stored in the pool.
    /// </summary>
    /// <returns>An array of all token instances.</returns>
    public T[] GetAllTokens()
    {
        return _GuidLookup.Values.ToArray();
    }

    /// <summary>
    /// Creates or retrieves a unique token instance for each non-empty, distinct name in the input list.
    /// </summary>
    /// <param name="names">A collection of token names to normalize and create.</param>
    /// <returns>An enumerable of token instances.</returns>
    public IEnumerable<T> GetOrCreateTokens(IEnumerable<string> names)
    {
        return names
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(Normalize)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(GetOrCreateToken);
    }

    /// <summary>
    /// Removes a token from the pool based on its <see cref="INamedToken.Id"/> and <see cref="INamedToken.Name"/>.
    /// </summary>
    /// <param name="token">The token to remove.</param>
    /// <returns><c>true</c> if the token was successfully removed; otherwise, <c>false</c>.</returns>
    public bool RemoveToken(T token)
    {
        ArgumentNullException.ThrowIfNull(token);

        var nameLock = _NameLocks.ObtainLockObject(token.Name);
        nameLock.EnterWriteLock();
        var guidLock = _GuidLocks.ObtainLockObject(token.Id);
        guidLock.EnterWriteLock();

        bool result = _GuidLookup.TryRemove(token.Id, out _) && _NameLookup.Remove(token.Name, out _);
        guidLock.ExitWriteLock();
        nameLock.ExitWriteLock();
        return result;
    }

    /// <summary>
    /// Gets an existing token by normalized name or creates a new one if it doesn't exist.
    /// </summary>
    /// <param name="name">The token name to lookup or create.</param>
    /// <returns>The existing or newly created token.</returns>
    /// <exception cref="ArgumentException">Thrown if the name is null, empty, or whitespace.</exception>
    public T GetOrCreateToken(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Token name cannot be empty.", nameof(name));

        string normalized = Normalize(name);

        if (_NameLookup.TryGetValue(normalized, out var existingToken))
            return existingToken;

        var nameLock = _NameLocks.ObtainLockObject(normalized);
        nameLock.EnterWriteLock();

        if (!_NameLookup.TryGetValue(normalized, out var token))
        {
            token = _factory(Guid.NewGuid(), normalized);
            _NameLookup[normalized] = token;
            _GuidLookup[token.Id] = token;
        }
        nameLock.ExitWriteLock();
        return token;
    }

    /// <summary>
    /// Inserts an existing token with the specified ID and name, if it's not already in the pool.
    /// </summary>
    /// <param name="token">The token to insert</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a token with the same name but different ID, or the same ID but different name, already exists.
    /// </exception>
    public void InsertToken(T token)

    {
        if (string.IsNullOrWhiteSpace(token.Name))
            throw new ArgumentException("Token name cannot be empty.", nameof(token.Name));
        if (token.Id == Guid.Empty)
            throw new ArgumentException("Token id cannot be empty.", nameof(token.Id));

        string normalized = Normalize(token.Name);

        var nameLock = _NameLocks.ObtainLockObject(normalized);
        var guidLock = _GuidLocks.ObtainLockObject(token.Id);

        nameLock.EnterWriteLock();
        guidLock.EnterWriteLock();

        try
        {
            // Check for conflict: same name, different ID
            if (_NameLookup.TryGetValue(normalized, out var existingByName))
            {
                if (existingByName.Id != token.Id)
                    throw new InvalidOperationException($"A token with name '{normalized}' already exists with a different ID.");
                return;
            }

            // Check for conflict: same ID, different name
            if (_GuidLookup.TryGetValue(token.Id, out var existingByGuid))
            {
                if (!string.Equals(existingByGuid.Name, normalized, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"A token with ID '{token.Id}' already exists with a different name '{existingByGuid.Name}'.");
                return;
            }

            // No conflicts – insert new token
            _NameLookup[normalized] = token;
            _GuidLookup[token.Id] = token;
            return;
        }
        finally
        {
            guidLock.ExitWriteLock();
            nameLock.ExitWriteLock();
        }
    }
    /// <summary>
    /// Inserts existing tokens with the specified ID and name, if they are not already in the pool.
    /// </summary>
    /// <param name="tokens">The tokens to insert</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a token with the same name but different ID, or the same ID but different name, already exists.
    /// </exception>
    public void InsertTokens(IEnumerable<T> tokens)

    {
        if (tokens == null) throw new ArgumentNullException(nameof(tokens));

        var tokenList = tokens.ToList(); // Ensure single enumeration
        if (tokenList.Count == 0) return;

        // Step 1: Validate and normalize tokens
        var normalizedTokens = new List<(string NormalizedName, T Token)>();
        foreach (var token in tokenList)
        {
            if (string.IsNullOrWhiteSpace(token.Name))
                throw new ArgumentException("Token name cannot be empty.", nameof(tokens));
            if (token.Id == Guid.Empty)
                throw new ArgumentException("Token id cannot be empty.", nameof(tokens));

            var normalized = Normalize(token.Name);
            normalizedTokens.Add((normalized, token));
        }
        // Step 2: Check for conflicts before locking
        foreach (var (normalized, token) in normalizedTokens)
        {
            if (_NameLookup.TryGetValue(normalized, out var existingByName) && existingByName.Id != token.Id)
                throw new InvalidOperationException($"A token with name '{normalized}' already exists with a different ID.");

            if (_GuidLookup.TryGetValue(token.Id, out var existingByGuid) &&
                !string.Equals(existingByGuid.Name, normalized, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"A token with ID '{token.Id}' already exists with a different name '{existingByGuid.Name}'.");
        }

        // Step 3: Lock once per unique name and ID, then insert
        var nameLocks = normalizedTokens.Select(t => _NameLocks.ObtainLockObject(t.NormalizedName)).Distinct().ToList();
        var guidLocks = normalizedTokens.Select(t => _GuidLocks.ObtainLockObject(t.Token.Id)).Distinct().ToList();

        foreach (var l in nameLocks) l.EnterWriteLock();
        foreach (var l in guidLocks) l.EnterWriteLock();

        try
        {
            foreach (var (normalized, token) in normalizedTokens)
            {
                _NameLookup[normalized] = token;
                _GuidLookup[token.Id] = token;
            }
        }
        finally
        {
            foreach (var l in guidLocks) l.ExitWriteLock();
            foreach (var l in nameLocks) l.ExitWriteLock();
        }
    }


    /// <summary>
    /// Attempts to retrieve a token by its unique <see cref="Guid"/> identifier.
    /// </summary>
    /// <param name="guid">The unique identifier of the token.</param>
    /// <returns>The token if found; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="guid"/> is empty.</exception>
    public T? GetToken(Guid guid)
    {
        if (guid == Guid.Empty)
            throw new ArgumentException("Guid cannot be empty.");

        return _GuidLookup.TryGetValue(guid, out var existingToken) ? existingToken : null;
    }

    /// <summary>
    /// Normalizes a token name by trimming whitespace.
    /// </summary>
    /// <param name="name">The name to normalize.</param>
    /// <returns>A normalized string.</returns>
    private static string Normalize(string name) => name.Trim();
}
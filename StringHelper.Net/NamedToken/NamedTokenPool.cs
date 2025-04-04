using System.Collections.Concurrent;
using Multithreading_Library.ThreadControl;

namespace StringHelper.Net.NamedToken;

public class NamedTokenPool<T> where T : class, INamedToken
{
    private readonly Func<Guid, string, T> _factory;
    private IDReaderWriterLocks<string> _NameLocks = new();
    private IDReaderWriterLocks<Guid> _GuidLocks = new();
    private ConcurrentDictionary<string, T> _NameLookup = new();
    private ConcurrentDictionary<Guid, T> _GuidLookup = new();

    public int Count => _NameLookup.Count;
    public NamedTokenPool(Func<Guid, string, T> factory)
    {
        _factory = factory;
    }
    public T[] GetAllTokens()
    {
        return _GuidLookup.Values.ToArray();
    }

    public IEnumerable<T> CreateTokens(IEnumerable<string> names)
    {
        return names
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(Normalize)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(GetOrCreateToken);
    }
    public bool RemoveToken(T token)
    {
        ArgumentNullException.ThrowIfNull(token);

        using var nameLock = _NameLocks.ObtainLockObject(token.Name);
        nameLock.EnterWriteLock();
        using var guidLock = _GuidLocks.ObtainLockObject(token.Id);
        guidLock.EnterWriteLock();

        return _GuidLookup.TryRemove(token.Id, out _) && _NameLookup.Remove(token.Name, out _);
    }

    public T GetOrCreateToken(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Token name cannot be empty.", nameof(name));

        string normalized = Normalize(name); // optional normalization

        if (_NameLookup.TryGetValue(normalized, out var token))
            return token;

        using var nameLock = _NameLocks.ObtainLockObject(normalized);
        nameLock.EnterWriteLock(); // upgrade
        token = _factory(Guid.NewGuid(), normalized);
        _NameLookup[normalized] = token;
        _GuidLookup[token.Id] = token;
        return token;
    }

    public T? GetToken(Guid guid)
    {
        if (guid == Guid.Empty)
            throw new ArgumentException("Guid cannot be empty.");

        return _GuidLookup.TryGetValue(guid, out var existingToken) ? existingToken : null;
    }
    private static string Normalize(string name) =>
        name.Trim().ToLowerInvariant();
}
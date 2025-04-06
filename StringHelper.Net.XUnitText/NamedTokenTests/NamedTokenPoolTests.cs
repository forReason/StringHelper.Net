using StringHelper.Net.NamedTokenNs;

namespace StringHelper.Net.XUnitText.NamedTokenTests;

public class NamedTokenPoolTests
{
    private NamedTokenPool<NamedTokenNs.NamedToken> CreatePool() =>
        new NamedTokenPool<NamedToken>((id, name) => new NamedToken(id, name));

    [Fact]
    public void GetOrCreateToken_ReturnsSameInstance_ForSameName()
    {
        var pool = new NamedTokenPool<NamedToken>((id, name) => new NamedToken(id, name));

        var token1 = pool.GetOrCreateToken("MyTag");
        var token2 = pool.GetOrCreateToken("MyTag");

        Assert.Same(token1, token2);
    }

    [Fact]
    public void GetOrCreateToken_TrimsAndIgnoresCase()
    {
        var pool = new NamedTokenPool<NamedToken>((id, name) => new NamedToken(id, name));

        var token1 = pool.GetOrCreateToken("  Important  ");
        var token2 = pool.GetOrCreateToken("important");

        Assert.Same(token1, token2);
        Assert.Equal("Important", token1.Name); // Original casing is preserved
    }

    [Fact]
    public void CreateTokens_RemovesDuplicates_IgnoresWhitespaceAndCase()
    {
        var pool = new NamedTokenPool<NamedToken>((id, name) => new NamedToken(id, name));

        var tokens = pool.GetOrCreateTokens(new[] { "A", " a ", "B", "b", "B " }).ToArray();

        Assert.Equal(2, tokens.Length);
        Assert.Contains(tokens, t => t.Name.Trim().Equals("A", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(tokens, t => t.Name.Trim().Equals("B", StringComparison.OrdinalIgnoreCase));
    }
     [Fact]
    public void InsertToken_InsertsNewToken_Successfully()
    {
        var pool = CreatePool();
        var token = pool.GetOrCreateToken(name: "InsertMe");

        pool.InsertToken(token);

        var retrieved = pool.GetToken(token.Id);
        Assert.NotNull(retrieved);
        Assert.Same(token, retrieved);
    }

    [Fact]
    public void InsertToken_SameNameAndId_NoConflict()
    {
        var pool = CreatePool();
        var token = pool.GetOrCreateToken(name: "Same");

        pool.InsertToken(token);
        pool.InsertToken(token); // should not throw

        Assert.Equal(1, pool.Count);
    }

    [Fact]
    public void InsertToken_ConflictingId_Throws()
    {
        var pool = CreatePool();
        var original = new NamedToken(Guid.NewGuid(), "Alpha");
        var conflict = new NamedToken(original.Id, "Beta"); // same ID, different name

        pool.InsertToken(original);
        var ex = Assert.Throws<InvalidOperationException>(() => pool.InsertToken(conflict));
        Assert.Contains("already exists with a different name", ex.Message);
    }

    [Fact]
    public void InsertToken_ConflictingName_Throws()
    {
        var pool = CreatePool();
        var original = new NamedToken(Guid.NewGuid(), "Shared");
        var conflict = new NamedToken(Guid.NewGuid(), "Shared"); // same name, different ID

        pool.InsertToken(original);
        var ex = Assert.Throws<InvalidOperationException>(() => pool.InsertToken(conflict));
        Assert.Contains("already exists with a different ID", ex.Message);
    }

    [Fact]
    public void InsertTokens_BatchInsert_WorksCorrectly()
    {
        var pool = CreatePool();

        var tokens = new[]
        {
            pool.GetOrCreateToken(name: "One"),
            pool.GetOrCreateToken(name: "Two"),
            pool.GetOrCreateToken(name: "Three")
        };

        pool.InsertTokens(tokens);

        foreach (var token in tokens)
        {
            var loaded = pool.GetToken(token.Id);
            Assert.NotNull(loaded);
            Assert.Same(token, loaded);
        }

        Assert.Equal(3, pool.Count);
    }

    [Fact]
    public void InsertTokens_WithConflictingToken_Throws()
    {
        var pool = CreatePool();

        var original = pool.GetOrCreateToken(name: "Item");
        pool.InsertToken(original);

        var conflicting = new NamedToken(Guid.NewGuid(), "Item"); // same name, different ID
        var batch = new[] { conflicting };

        var ex = Assert.Throws<InvalidOperationException>(() => pool.InsertTokens(batch));
        Assert.Contains("already exists with a different ID", ex.Message);
    }

    [Fact]
    public void InsertToken_ThrowsOnEmptyName()
    {
        var pool = CreatePool();
        var token = new NamedToken(Guid.NewGuid(), "  ");

        var ex = Assert.Throws<ArgumentException>(() => pool.InsertToken(token));
        Assert.Contains("Token name cannot be empty", ex.Message);
    }

    [Fact]
    public void InsertToken_ThrowsOnEmptyGuid()
    {
        var pool = CreatePool();
        var token = new NamedToken(Guid.Empty, "ValidName");

        var ex = Assert.Throws<ArgumentException>(() => pool.InsertToken(token));
        Assert.Contains("Token id cannot be empty", ex.Message);
    }

    [Fact]
    public void InsertTokens_ThrowsOnNull()
    {
        var pool = CreatePool();

        Assert.Throws<ArgumentNullException>(() => pool.InsertTokens(null!));
    }

    [Fact]
    public void InsertTokens_IgnoresEmptyEnumerable()
    {
        var pool = CreatePool();
        pool.InsertTokens(Array.Empty<NamedToken>());

        Assert.Equal(0, pool.Count);
    }

    [Fact]
    public void RemoveToken_RemovesSuccessfully()
    {
        var pool = new NamedTokenPool<NamedToken>((id, name) => new NamedToken(id, name));

        var token = pool.GetOrCreateToken("ToRemove");
        var removed = pool.RemoveToken(token);

        Assert.True(removed);
        Assert.Equal(0, pool.Count);
    }

    [Fact]
    public void GetToken_ReturnsTokenById()
    {
        var pool = new NamedTokenPool<NamedToken>((id, name) => new NamedToken(id, name));

        var token = pool.GetOrCreateToken("ByIdTest");
        var retrieved = pool.GetToken(token.Id);

        Assert.Same(token, retrieved);
    }

    [Fact]
    public void GetToken_ThrowsForEmptyGuid()
    {
        var pool = CreatePool();

        Assert.Throws<ArgumentException>(() => pool.GetToken(Guid.Empty));
    }

    [Fact]
    public void GetOrCreateToken_ThrowsForEmptyName()
    {
        var pool = CreatePool();

        Assert.Throws<ArgumentException>(() => pool.GetOrCreateToken("   "));
    }

    [Fact]
    public void GetAllTokens_ReturnsAllStoredTokens()
    {
        var pool = new NamedTokenPool<NamedToken>((id, name) => new NamedToken(id, name));

        var t1 = pool.GetOrCreateToken("One");
        var t2 = pool.GetOrCreateToken("Two");

        var all = pool.GetAllTokens();

        Assert.Contains(t1, all);
        Assert.Contains(t2, all);
        Assert.Equal(2, all.Length);
    }
    [Fact]
    public void RemoveToken_WithNonExistentGuid_ReturnsFalse()
    {
        var pool = new NamedTokenPool<NamedToken>((id, name) => new NamedToken(id, name));

        // Arrange: Add one token
        var existing = pool.GetOrCreateToken("Present");
        Assert.Equal(1, pool.Count);

        // Create a token that mimics a real one, but with a unique unused Guid
        var fakeToken = new NamedToken(Guid.NewGuid(), "Present"); // Same name, different ID

        // Act
        bool removed = pool.RemoveToken(fakeToken);

        // Assert
        Assert.False(removed); // Removal should fail
        Assert.Equal(1, pool.Count); // Nothing was actually removed
        Assert.Same(existing, pool.GetOrCreateToken("Present")); // Still in pool
    }

    [Fact]
    public void Concurrent_GetOrCreate_MultipleNames_CountAccurate()
    {
        var pool = CreatePool();
        int uniqueNameCount = 100;
        int totalOperations = 1000;
        string[] names = Enumerable.Range(0, uniqueNameCount)
            .Select(i => $"Name{i}")
            .ToArray();

        // Act: concurrently request tokens for various names in parallel
        Parallel.For(0, totalOperations, i =>
        {
            string name = names[i % uniqueNameCount];
            pool.GetOrCreateToken(name);
        });

        // Assert: pool count should equal the number of unique names
        Assert.Equal(uniqueNameCount, pool.Count);

        // Each expected name should be present exactly once (verifying no missed tokens)
        foreach (string name in names)
        {
            int countBefore = pool.Count;
            var token = pool.GetOrCreateToken(name);  // should retrieve existing token, not create new
            Assert.Equal(name, token.Name);
            Assert.Equal(countBefore, pool.Count);    // Count remains unchanged, token was already in pool
        }
    }

    [Fact]
    public void MultipleThreads_GetOrCreate_SameName_ReturnsSameToken()
    {
        var pool = CreatePool();
        string tokenName = "TestToken";
        int threadCount = 100;
        NamedToken[] results = new NamedToken[threadCount];

        // Act: perform concurrent GetOrCreateToken calls with the same name
        Parallel.For(0, threadCount, i =>
        {
            results[i] = pool.GetOrCreateToken(tokenName);
        });

        // Assert: all results should point to the same instance
        NamedToken firstToken = results[0];
        Assert.NotNull(firstToken);
        Assert.Equal(tokenName, firstToken.Name);
        Assert.All(results, token => Assert.Same(firstToken, token));

        // Only one unique token should exist in the pool
        Assert.Equal(1, pool.Count);
    }
    [Fact]
    public void Concurrent_RemoveToken_NoExceptionsAndConsistentState()
    {
        var pool = CreatePool();
        int initialCount = 100;

        // Arrange: Add 100 unique tokens and capture them
        var tokens = Enumerable.Range(0, initialCount)
            .Select(i => pool.GetOrCreateToken($"Name{i}"))
            .ToArray();

        Assert.Equal(initialCount, pool.Count);

        // Create a token list that includes duplicate removals
        var duplicateTokenList = tokens.Concat(tokens).ToList(); // 2x each token

        // Act: Remove tokens concurrently
        Parallel.ForEach(duplicateTokenList, token =>
        {
            // Concurrent calls to RemoveToken – only one should succeed per token
            pool.RemoveToken(token);
        });

        // Assert: all tokens are removed, count is zero
        Assert.Equal(0, pool.Count);

        // Ensure pool remains usable
        var newToken = pool.GetOrCreateToken("AfterRemoval");
        Assert.Equal("AfterRemoval", newToken.Name);
        Assert.Equal(1, pool.Count);
    }


}
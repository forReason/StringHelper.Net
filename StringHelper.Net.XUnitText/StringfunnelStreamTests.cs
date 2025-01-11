namespace StringHelper.Net.XUnitText;

public class StringFunnelStreamTests
{
    [Fact]
    public void Match_ReturnsTrue_WhenSingleChunkMatchesFilter()
    {
        // Arrange
        string[] filters = { "hello", "world", "example" };
        StringFunnelStream stream = new StringFunnelStream(filters);

        // Act
        bool match = stream.Process("This is a hello world example.", StringFunnel.MatchType.Contains);

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_ReturnsFalse_WhenChunkDoesNotMatchFilter()
    {
        // Arrange
        string[] filters = { "hello", "world", "example" };
        StringFunnelStream stream = new StringFunnelStream(filters);

        // Act
        bool match = stream.Process("This is a test string.", StringFunnel.MatchType.Contains);

        // Assert
        Assert.False(match);
    }

    [Fact]
    public void Match_HandlesChunksAcrossMultipleCalls()
    {
        // Arrange
        string[] filters = { "stream", "processing" };
        StringFunnelStream stream = new StringFunnelStream(filters);

        // Act
        bool matchPart1 = stream.Process("This is a test for stream ", StringFunnel.MatchType.Contains);
        bool matchPart2 = stream.Process("processing in chunks.", StringFunnel.MatchType.Contains);

        // Assert
        Assert.True(matchPart1 || matchPart2);
    }

    [Fact]
    public void Clear_ResetsStreamState()
    {
        // Arrange
        string[] filters = { "reset", "test" };
        StringFunnelStream stream = new StringFunnelStream(filters);

        // Act
        bool matchBeforeClear = stream.Process("This is a reset test.", StringFunnel.MatchType.Contains);
        stream.Clear();
        bool matchAfterClear = stream.Process("This is a reset test.", StringFunnel.MatchType.Contains);

        // Assert
        Assert.False(matchAfterClear);
    }

    [Fact]
    public void AddFilter_ProcessesNewlyAddedFilters()
    {
        // Arrange
        string[] filters = { "initial", "filter" };
        StringFunnelStream stream = new StringFunnelStream(filters);
        stream.AddFilter("newfilter");

        // Act
        bool match = stream.Process("This includes a newfilter.", StringFunnel.MatchType.Contains);

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_ReturnsTrue_ForBeginsWithMatch()
    {
        // Arrange
        string[] filters = { "begin" };
        StringFunnelStream stream = new StringFunnelStream(filters);

        // Act
        bool match = stream.Process("Begin with this text.", StringFunnel.MatchType.BeginsWith);

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_ReturnsFalse_WhenNoBeginsWithMatch()
    {
        // Arrange
        string[] filters = { "begin" };
        StringFunnelStream stream = new StringFunnelStream(filters);

        // Act
        bool match = stream.Process("Does not begin with the word.", StringFunnel.MatchType.BeginsWith);

        // Assert
        Assert.False(match);
    }

    [Fact]
    public void Process_HandlesEmptyChunksGracefully()
    {
        // Arrange
        string[] filters = { "test" };
        StringFunnelStream stream = new StringFunnelStream(filters);

        // Act
        bool match = stream.Process("", StringFunnel.MatchType.Contains);

        // Assert
        Assert.False(match);
    }

    [Fact]
    public void Process_HandlesDelimitersCorrectly()
    {
        // Arrange
        string[] filters = { "hello", "world" };
        StringFunnelStream stream = new StringFunnelStream(filters);

        // Act
        bool match = stream.Process("Hello, world!", StringFunnel.MatchType.Contains);

        // Assert
        Assert.True(match);
    }
}
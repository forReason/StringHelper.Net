using StringHelper.Net.Filtering.WordFilters;
using MatchType = StringHelper.Net.Filtering.WordFilters.MatchType;

namespace StringHelper.Net.XUnitText;

public class WordFilterStreamTests
{
    [Fact]
    public void Match_GeneratewordFilter()
    {
        // Arrange
        WordFilter test = new WordFilter(["foo", "bar"]);
        WordFilterStream stream = new WordFilterStream(test);
        WordFilterStream stream2 = test.GetFilterStream();

        // Act
        bool match = stream.Process("This is a hello world example.", MatchType.Contains);

        // Assert
        Assert.True(match);
    }
    [Fact]
    public void Match_ReturnsTrue_WhenSingleChunkMatchesFilter()
    {
        // Arrange
        string[] filters = { "hello", "world", "example" };
        WordFilterStream stream = new WordFilterStream(filters);

        // Act
        bool match = stream.Process("This is a hello world example.", MatchType.Contains);

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_ReturnsFalse_WhenChunkDoesNotMatchFilter()
    {
        // Arrange
        string[] filters = { "hello", "world", "example" };
        WordFilterStream stream = new WordFilterStream(filters);

        // Act
        bool match = stream.Process("This is a test string.", MatchType.Contains);

        // Assert
        Assert.False(match);
    }

    [Fact]
    public void Match_HandlesChunksAcrossMultipleCalls()
    {
        // Arrange
        string[] filters = { "stream", "processing" };
        WordFilterStream stream = new WordFilterStream(filters);

        // Act
        bool matchPart1 = stream.Process("This is a test for stream ", MatchType.Contains);
        bool matchPart2 = stream.Process("processing in chunks.", MatchType.Contains);

        // Assert
        Assert.True(matchPart1 || matchPart2);
    }

    [Fact]
    public void Clear_ResetsStreamState()
    {
        // Arrange
        string[] filters = { "reset test" };
        WordFilterStream stream = new WordFilterStream(filters);

        // Act
        bool matchBeforeClear = stream.Process("This is a reset test.", MatchType.Contains);
        stream.Clear();
        stream.AddFilter("this is");
        bool matchAfterClear = stream.Process("This is a reset test.", MatchType.BeginsWith);

        // Assert
        Assert.True(matchAfterClear);
    }

    [Fact]
    public void AddFilter_ProcessesNewlyAddedFilters()
    {
        // Arrange
        string[] filters = { "initial", "filter" };
        WordFilterStream stream = new WordFilterStream(filters);
        stream.AddFilter("newfilter");

        // Act
        bool match = stream.Process("This includes a newfilter.", MatchType.Contains);

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_ReturnsTrue_ForBeginsWithMatch()
    {
        // Arrange
        string[] filters = { "begin" };
        WordFilterStream stream = new WordFilterStream(filters);

        // Act
        bool match = stream.Process("Begin with this text.", MatchType.BeginsWith);

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_ReturnsFalse_WhenNoBeginsWithMatch()
    {
        // Arrange
        string[] filters = { "begin" };
        WordFilterStream stream = new WordFilterStream(filters);

        // Act
        bool match = stream.Process("Does not begin with the word.", MatchType.BeginsWith);

        // Assert
        Assert.False(match);
    }

    [Fact]
    public void Process_HandlesEmptyChunksGracefully()
    {
        // Arrange
        string[] filters = { "test" };
        WordFilterStream stream = new WordFilterStream(filters);

        // Act
        bool match = stream.Process("", MatchType.Contains);

        // Assert
        Assert.False(match);
    }

    [Fact]
    public void Process_HandlesDelimitersCorrectly()
    {
        // Arrange
        string[] filters = { "hello", "world" };
        WordFilterStream stream = new WordFilterStream(filters);

        // Act
        bool match = stream.Process("Hello, world!", MatchType.Contains);

        // Assert
        Assert.True(match);
    }
    [Fact]
    public void Process_Multi_test()
    {
        // Arrange
        string[] filters =
        {
            "I can't",
            "I can not",
            "I Won't"
        };
        WordFilterStream stream = new WordFilterStream(filters);

        // Act
        Assert.True(stream.Process("I cant do this", MatchType.BeginsWith));
        stream.Clear();
        Assert.True(stream.Process("I can not do this", MatchType.BeginsWith));
        stream.Clear();
        Assert.True(stream.Process("I wont do this", MatchType.BeginsWith));
        stream.Clear();
        Assert.False(stream.Process("i ", MatchType.BeginsWith));
        Assert.False(stream.Process("can ", MatchType.BeginsWith));
        Assert.True(stream.Process("not ", MatchType.BeginsWith));
    }
}
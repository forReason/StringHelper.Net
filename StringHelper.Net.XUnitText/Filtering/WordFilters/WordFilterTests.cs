using StringHelper.Net.Filtering.WordFilters;
using MatchType = StringHelper.Net.Filtering.WordFilters.MatchType;

namespace StringHelper.Net.XUnitText;

public class StringFunnelTests
{
    [Fact]
    public void Match_ReturnsTrue_ForExactMatch()
    {
        // Arrange
        string[] filters = { "hello", "world" };
        WordFilter funnel = new WordFilter(filters);

        // Act
        bool match = funnel.Match("hello");

        // Assert
        Assert.True(match);
    }
    [Fact]
    public void TestDelimiter()
    {

        char c = '’';
            bool delim =  c != '’' && c != '\'' && (
                char.IsWhiteSpace(c)
                || char.IsPunctuation(c)
                || char.IsSeparator(c)
            );
            bool isdelimiter = WordFilter.IsDelimiter('’');
        Assert.False(isdelimiter);
    }

    [Fact]
    public void Match_ReturnsTrue_ForContainsMatch()
    {
        // Arrange
        string[] filters = { "hello world" };
        WordFilter funnel = new WordFilter(filters);

        // Act
        bool match = funnel.Match("This is a hello world example.");

        // Assert
        Assert.True(match);
    }
    [Fact]
    public void Match_ReturnsFalse_ForNotMatch()
    {
        // Arrange
        string[] filters = { "hello world" };
        WordFilter funnel = new WordFilter(filters);

        // Act
        bool match = funnel.Match("This is a hello in world example.");

        // Assert
        Assert.False(match);
    }

    [Fact]
    public void Match_ReturnsFalse_WhenNoMatchFound()
    {
        // Arrange
        string[] filters = { "hello", "world" };
        WordFilter funnel = new WordFilter(filters);

        // Act
        bool match = funnel.Match("This string does not match any filters.");

        // Assert
        Assert.False(match);
    }

    [Fact]
    public void Match_ReturnsTrue_ForBeginsWithMatch()
    {
        // Arrange
        string[] filters = { "begin here" };
        WordFilter funnel = new WordFilter(filters);

        // Act
        bool match = funnel.Match("Begin here to test.", MatchType.BeginsWith);

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_ReturnsFalse_WhenNoBeginsWithMatch()
    {
        // Arrange
        string[] filters = { "begin", "start" };
        WordFilter funnel = new WordFilter(filters);

        // Act
        bool match = funnel.Match("This does not begin with a filter.", MatchType.BeginsWith);

        // Assert
        Assert.False(match);
    }

    [Fact]
    public void AddFilter_AllowsDynamicFilterAddition()
    {
        // false filter
        string[] filters = { "dynamic" };
        WordFilter funnel = new WordFilter(filters);
        bool match = funnel.Match("This has an additional filter.");
        Assert.False(match);

        // additional filter
        funnel.AddFilter("additional");
        match = funnel.Match("This has an additional filter.");
        Assert.True(match);
    }

    [Fact]
    public void Match_HandlesComplexInputsWithDelimiters()
    {
        // Arrange
        string[] filters = { "hello world" };
        WordFilter funnel = new WordFilter(filters);

        // Act
        bool match = funnel.Match("Hello, world!");

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_HandlesCaseInsensitiveMatching()
    {
        // Arrange
        string[] filters = { "case insensitive" };
        WordFilter funnel = new WordFilter(filters);

        // Act
        bool match = funnel.Match("This is CASE insensitive.");

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_ReturnsFalse_ForEmptyInput()
    {
        // Arrange
        string[] filters = { "test" };
        WordFilter funnel = new WordFilter(filters);

        // Act
        bool match = funnel.Match("");

        // Assert
        Assert.False(match);
    }

    [Fact]
    public void Match_ReturnsFalse_WhenNoFiltersAreSet()
    {
        // Arrange
        string[] filters = { };
        WordFilter funnel = new WordFilter(filters);

        // Act
        bool match = funnel.Match("This input won't match.");

        // Assert
        Assert.False(match);
    }
}
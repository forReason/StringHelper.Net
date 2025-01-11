namespace StringHelper.Net.XUnitText;

public class StringFunnelTests
{
    [Fact]
    public void Match_ReturnsTrue_ForExactMatch()
    {
        // Arrange
        string[] filters = { "hello", "world" };
        StringFunnel funnel = new StringFunnel(filters);

        // Act
        bool match = funnel.Match("hello");

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_ReturnsTrue_ForContainsMatch()
    {
        // Arrange
        string[] filters = { "hello", "world" };
        StringFunnel funnel = new StringFunnel(filters);

        // Act
        bool match = funnel.Match("This is a hello world example.");

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_ReturnsFalse_WhenNoMatchFound()
    {
        // Arrange
        string[] filters = { "hello", "world" };
        StringFunnel funnel = new StringFunnel(filters);

        // Act
        bool match = funnel.Match("This string does not match any filters.");

        // Assert
        Assert.False(match);
    }

    [Fact]
    public void Match_ReturnsTrue_ForBeginsWithMatch()
    {
        // Arrange
        string[] filters = { "begin", "start" };
        StringFunnel funnel = new StringFunnel(filters);

        // Act
        bool match = funnel.Match("Begin here to test.", StringFunnel.MatchType.BeginsWith);

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_ReturnsFalse_WhenNoBeginsWithMatch()
    {
        // Arrange
        string[] filters = { "begin", "start" };
        StringFunnel funnel = new StringFunnel(filters);

        // Act
        bool match = funnel.Match("This does not begin with a filter.", StringFunnel.MatchType.BeginsWith);

        // Assert
        Assert.False(match);
    }

    [Fact]
    public void AddFilter_AllowsDynamicFilterAddition()
    {
        // Arrange
        string[] filters = { "dynamic" };
        StringFunnel funnel = new StringFunnel(filters);
        funnel.AddFilter("additional");

        // Act
        bool match = funnel.Match("This has an additional filter.");

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_HandlesComplexInputsWithDelimiters()
    {
        // Arrange
        string[] filters = { "hello", "world" };
        StringFunnel funnel = new StringFunnel(filters);

        // Act
        bool match = funnel.Match("Hello, world!");

        // Assert
        Assert.True(match);
    }

    [Fact]
    public void Match_HandlesCaseInsensitiveMatching()
    {
        // Arrange
        string[] filters = { "case", "insensitive" };
        StringFunnel funnel = new StringFunnel(filters);

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
        StringFunnel funnel = new StringFunnel(filters);

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
        StringFunnel funnel = new StringFunnel(filters);

        // Act
        bool match = funnel.Match("This input won't match.");

        // Assert
        Assert.False(match);
    }
}
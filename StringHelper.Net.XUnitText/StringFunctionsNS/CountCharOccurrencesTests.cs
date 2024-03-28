using StringHelper.Net.StringFunctionsNS; // Adjust the namespace to match where CountCharOccurrences is located

public class CountCharOccurrencesTests
{
    [Fact]
    public void CountCharOccurrences_NullInput_CountsCorrectly()
    {
        // Arrange
        var stringFunctions = new CountCharOccurrences();
        string input = null;

        // Act
        var result = stringFunctions.Evaluate(input, CountCharOccurrences.SortOption.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void CountCharOccurrences_WithEmptyString_ReturnsEmptyDictionary()
    {
        // Arrange
        var stringFunctions = new CountCharOccurrences();
        string input = "";

        // Act
        var result = stringFunctions.Evaluate(input, CountCharOccurrences.SortOption.None);

        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void CountCharOccurrences_WithDefaultSortingWorks()
    {
        // Arrange
        var stringFunctions = new CountCharOccurrences();
        string input = "aazzbb";

        // Act
        var result = stringFunctions.Evaluate(input, default).ToList();

        // Assert
        Assert.Equal('a', result[0].Key);
        Assert.Equal('z', result[1].Key);
        Assert.Equal('b', result[2].Key);
    }
    
    [Fact]
    public void ReevaluatingDoesNotModifyArray()
    {
        // Arrange
        var stringFunctions = new CountCharOccurrences();
        string input = "aabbcc";

        // Act
        var result = stringFunctions.Evaluate(input, CountCharOccurrences.SortOption.None);
        var result2 = stringFunctions.Evaluate("DEF", CountCharOccurrences.SortOption.None);

        // Assert
        Assert.Equal(2, result['a']);
        Assert.Equal(2, result['b']);
        Assert.Equal(2, result['c']);
        Assert.Equal(1,result2['D']);
        Assert.Equal(1,result2['E']);
        Assert.Equal(1,result2['F']);
        Assert.Equal(3, result.Count); // Ensure only 3 characters are counted
    }

    [Fact]
    public void CountCharOccurrences_WithValidInput_CountsCorrectly()
    {
        // Arrange
        var stringFunctions = new CountCharOccurrences();
        string input = "aabbbc";

        // Act
        var result = stringFunctions.Evaluate(input, CountCharOccurrences.SortOption.None);

        // Assert
        Assert.Equal(2, result['a']);
        Assert.Equal(3, result['b']);
        Assert.Equal(1, result['c']);
        Assert.Equal(3, result.Count); // Ensure only 3 characters are counted
    }
    
    [Fact]
    public void CountCharOccurrences_SortedAlphabeticallyAscending_ReturnsSortedDictionary()
    {
        // Arrange
        var stringFunctions = new CountCharOccurrences();
        string input = "cba2cb1";

        // Act
        var result = stringFunctions.Evaluate(input, CountCharOccurrences.SortOption.AlphabeticallyAscending).ToList();

        // Assert
        Assert.Equal('1', result[0].Key);
        Assert.Equal('2', result[1].Key);
        Assert.Equal('a', result[2].Key);
        Assert.Equal('b', result[3].Key);
        Assert.Equal('c', result[4].Key);
    }

    [Fact]
    public void CountCharOccurrences_SortedAlphabeticallyDescending_ReturnsSortedDictionary()
    {
        // Arrange
        var stringFunctions = new CountCharOccurrences();
        string input = "1cbac3b";

        // Act
        var result = stringFunctions.Evaluate(input, CountCharOccurrences.SortOption.AlphabeticallyDescending).ToList();

        // Assert
        Assert.Equal('c', result[0].Key);
        Assert.Equal('b', result[1].Key);
        Assert.Equal('a', result[2].Key);
        Assert.Equal('3', result[3].Key);
        Assert.Equal('1', result[4].Key);
    }
    [Fact]
    public void CountCharOccurrences_SortedNumericallyAscending_ReturnsSortedDictionary()
    {
        // Arrange
        var stringFunctions = new CountCharOccurrences();
        string input = "abbbcc";

        // Act
        var result = stringFunctions.Evaluate(input, CountCharOccurrences.SortOption.CharOccurrencesAscending).ToList();

        // Assert
        Assert.Equal('a', result[0].Key);
        Assert.Equal('c', result[1].Key);
        Assert.Equal('b', result[2].Key);
    }
    [Fact]
    public void CountCharOccurrences_SortedNumericallyDescending_ReturnsSortedDictionary()
    {
        // Arrange
        var stringFunctions = new CountCharOccurrences();
        string input = "abbbcc";

        // Act
        var result = stringFunctions.Evaluate(input, CountCharOccurrences.SortOption.CharOccurrencesDescending).ToList();

        // Assert
        Assert.Equal('b', result[0].Key);
        Assert.Equal('c', result[1].Key);
        Assert.Equal('a', result[2].Key);
    }
    [Fact]
    public void CountCharOccurrences_RenderString()
    {
        // Arrange
        var stringFunctions = new CountCharOccurrences();
        string input = "aaabbccccd";

        // Act
        var result = stringFunctions.Evaluate(input, CountCharOccurrences.SortOption.CharOccurrencesDescending);

        // Assert
        Assert.Equal("a3b2c4d1", stringFunctions.ToString());
    }
    [Fact]
    public void CountCharOccurrences_RenderEmptyString()
    {
        // Arrange
        var stringFunctions = new CountCharOccurrences();

        // Assert
        Assert.Equal("", stringFunctions.ToString());
    }
}

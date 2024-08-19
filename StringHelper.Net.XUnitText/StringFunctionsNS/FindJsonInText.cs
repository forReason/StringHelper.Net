namespace StringHelper.Net.XUnitText.StringFunctionsNS;

public class FindJsonInText
{
    [Fact]
    public void FindJsonInText_ShouldReturnJson_WhenInputContainsValidJson()
    {
        // Arrange
        string input = "Here is a JSON: {\"title\": \"Example\", \"done\": false} and some text after.";
        var jsonFinder = new StringFunctions();

        // Act
        string? result = jsonFinder.FindJsonInText(ref input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("{\"title\": \"Example\", \"done\": false}", result);
    }

    [Fact]
    public void FindJsonInText_ShouldReturnFirstJson_WhenInputContainsMultipleJsonObjects()
    {
        // Arrange
        string input = "First JSON: {\"title\": \"First\", \"done\": false} Second JSON: {\"title\": \"Second\", \"done\": true}";
        var jsonFinder = new StringFunctions();

        // Act
        string? result = jsonFinder.FindJsonInText(ref input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("{\"title\": \"First\", \"done\": false}", result);
    }

    [Fact]
    public void FindJsonInText_ShouldReturnNull_WhenInputContainsNoJson()
    {
        // Arrange
        string input = "This text does not contain any JSON object.";
        var jsonFinder = new StringFunctions();

        // Act
        string? result = jsonFinder.FindJsonInText(ref input);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FindJsonInText_ShouldReturnNull_WhenInputContainsIncompleteJson()
    {
        // Arrange
        string input = "Here is an incomplete JSON: {\"title\": \"Incomplete\", \"done\": false";
        var jsonFinder = new StringFunctions();

        // Act
        string? result = jsonFinder.FindJsonInText(ref input);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FindJsonInText_ShouldReturnNestedJson_WhenInputContainsNestedJson()
    {
        // Arrange
        string input = "Here is a nested JSON: {\"title\": \"Parent\", \"child\": {\"name\": \"Child\"}}";
        var jsonFinder = new StringFunctions();

        // Act
        string? result = jsonFinder.FindJsonInText(ref input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("{\"title\": \"Parent\", \"child\": {\"name\": \"Child\"}}", result);
    }

    [Fact]
    public void FindJsonInText_ShouldReturnNull_WhenInputIsEmpty()
    {
        // Arrange
        string input = "";
        var jsonFinder = new StringFunctions();

        // Act
        string? result = jsonFinder.FindJsonInText(ref input);

        // Assert
        Assert.Null(result);
    }
}
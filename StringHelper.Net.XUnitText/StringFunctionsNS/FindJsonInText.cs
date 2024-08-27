using System.Diagnostics;

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

    [Theory]
    [InlineData("{\"cardNeedsUpdate\": false, \"additionalDetails\": \"\"}", "{\"cardNeedsUpdate\": false, \"additionalDetails\": \"\"}")]
    [InlineData("Here's my next response, compiled according to the provided JSON schema:\n\n```json\n{\n  \"answer\": \"I'd like to confirm that I understand the project requirements. To clarify, you want an image generator that can produce various digital PNG images based on text prompts, such as 'a small cat holding up a sign stating \"hello world\".'? Is this correct?\",\n  \"justification\": \"To ensure understanding of the project requirements and provide accurate responses going forward.\"\n}\n```\n\nThis response addresses the customer with \"Dear Customer,\" to initiate a conversation about the project details. It also references the 'Initial Tasks' checklist, which outlines the necessary information to be gathered from the customer.\n\nIf you'd like me to proceed further, please let me know what's next!", "{\n  \"answer\": \"I'd like to confirm that I understand the project requirements. To clarify, you want an image generator that can produce various digital PNG images based on text prompts, such as 'a small cat holding up a sign stating \"hello world\".'? Is this correct?\",\n  \"justification\": \"To ensure understanding of the project requirements and provide accurate responses going forward.\"\n}")]
    [InlineData("```\n{\"cardNeedsUpdate\": false, \"additionalDetails\": \"\"}\n```", "{\"cardNeedsUpdate\": false, \"additionalDetails\": \"\"}")]
    [InlineData("```\n{\n  \"answer\": \"Dear Customer, thank you for clarifying the details. Based on your preferences, I would suggest having the party in a garden or park in Basel, Switzerland that is well-maintained and has no issues with mosquitoes, horseflies, and wasps. This way, we can ensure a pleasant atmosphere for your guests.\n\nRegarding the game 'Werwölfe von Düsterwald', I'll make sure to include it in our planning. If you'd like, we could also consider setting up some outdoor games or activities that fit with the casual atmosphere you're looking for.\n\nTo confirm, I've updated the initial tasks checklist as follows:\n\n* The project's title is short and clearly understandable: [mark as done]\n* The project's description is clearly understandable: [no changes needed]\n* The project's main objective is clearly defined: [mark as done]\n* The party will be held in a garden or park: [new task, marked as not started]\n* The party will have a casual atmosphere: [new task, marked as not started]\n* The game 'Werwölfe von Düsterwald' will be included: [new task, marked as not started]\n\nPlease review these changes and let me know if you'd like to add or modify anything.\n\nJustification: Clarified project details and updated the initial tasks checklist accordingly.\",\n  \"justification\": \"Ensured all necessary information was gathered before proceeding with planning.\"\n}\n```", "{\n  \"answer\": \"Dear Customer, thank you for clarifying the details. Based on your preferences, I would suggest having the party in a garden or park in Basel, Switzerland that is well-maintained and has no issues with mosquitoes, horseflies, and wasps. This way, we can ensure a pleasant atmosphere for your guests.\n\nRegarding the game 'Werwölfe von Düsterwald', I'll make sure to include it in our planning. If you'd like, we could also consider setting up some outdoor games or activities that fit with the casual atmosphere you're looking for.\n\nTo confirm, I've updated the initial tasks checklist as follows:\n\n* The project's title is short and clearly understandable: [mark as done]\n* The project's description is clearly understandable: [no changes needed]\n* The project's main objective is clearly defined: [mark as done]\n* The party will be held in a garden or park: [new task, marked as not started]\n* The party will have a casual atmosphere: [new task, marked as not started]\n* The game 'Werwölfe von Düsterwald' will be included: [new task, marked as not started]\n\nPlease review these changes and let me know if you'd like to add or modify anything.\n\nJustification: Clarified project details and updated the initial tasks checklist accordingly.\",\n  \"justification\": \"Ensured all necessary information was gathered before proceeding with planning.\"\n}")]
    [InlineData("{\n  \"actions\": [\n    \"edit_card\",\n    \"write_comment\"\n  ]\n}", "{\n  \"actions\": [\n    \"edit_card\",\n    \"write_comment\"\n  ]\n}")]
    public void FindJsonInText_ShouldMatchExpectedJson_WhenValidJsonWithSpecificStructureIsProvided(string input, string expected)
    {
        // Arrange
        var jsonFinder = new StringFunctions();

        // Act
        string? result = jsonFinder.FindJsonInText(ref input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

}
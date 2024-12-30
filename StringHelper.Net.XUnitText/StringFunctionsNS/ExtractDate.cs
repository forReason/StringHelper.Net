namespace StringHelper.Net.XUnitText.StringFunctionsNS;

public class ExtractDate
{
    [Fact]
    public void ExtractDate_ValidDate_ReturnsCorrectDate()
    {
        // Arrange
        var input = "Release Date : 04/26/2024";
        var expectedDate = new DateTime(2024, 4, 26);
            
        // Act
        var result = StringFunctions.ExtractDate(input);
            
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDate, result.Value);
    }
        
    [Fact]
    public void ExtractDate_InvalidDate_ReturnsNull()
    {
        // Arrange
        var input = "Release Date : invalid date";
            
        // Act
        var result = StringFunctions.ExtractDate(input);
            
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ExtractDate_EmptyString_ReturnsNull()
    {
        // Arrange
        var input = "";
            
        // Act
        var result = StringFunctions.ExtractDate(input);
            
        // Assert
        Assert.Null(result);
    }
}
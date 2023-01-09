# StringHelper.Net
StringHelper.Net is a .NET library that provides helpful functions for working with strings. It contains the following classes:

- InvariantString: Provides functions for cleaning and normalizing strings that may have come from user input.
- NumberFormatting: Converts numbers (such as currency) to appropriate string outputs.
- StringFunctions: Provides functions for quick matching, trimming, and line counting.
- TextFileFunctions: Allows for converting file encodings and fetching a specific number of lines from a text file.
- UTF8_Quickdecode: Converts a UTF8 encoded char.

# Usage
To use StringHelper.Net in your project, install it via NuGet:
```
Install-Package StringHelper.Net
```

You can then use the various functions provided by the library in your code. Here are a few examples:
```
using StringHelper.Net;

public void InvaryString_Test()
{
    string sample = " Alföns Frènke ";
    string result = InvariantString.InvaryString(sample, true);
    if (result != "alfoens frenke")
    {
        throw new Exception($"{result} should be alfoens frenke");
    }
}

public void RemoveCharacters()
{
    string sample = " Alföns Frènke. Öpper Drüpper";
    string result = InvariantString.RemoveCharacters(sample, new[] {'ö','p'} );
    if (result != " Alfns Frènke. Öer Drüer")
    {
        throw new Exception($"'{result}' should be ' Alfns Frènke. Öer Drüer'");
    }
}
```

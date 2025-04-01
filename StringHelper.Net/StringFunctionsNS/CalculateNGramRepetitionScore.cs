using System.Text.RegularExpressions;

namespace StringHelper.Net.StringFunctionsNS;

public class CalculateNGramRepetitionScore
{
    /// <summary>
    /// returns a score between 0.0 (no repetition) to 1.0 (only repetitions)<br/>
    /// eg:<br/>
    /// - No repetition at all: 0.0<br/>
    /// - Some repetition: 0.3<br/>
    /// - Lots of repeated content: 0.75<br/>
    /// - Fully identical content: 1.0
    /// </summary>
    /// <param name="text">the text to analyze</param>
    /// <param name="n">the number of words to group to an n-gram</param>
    /// <returns></returns>
    public static (double Score, List<(string Phrase, int Count)> RepeatingPhrases) ComputeNGramRepetitionScore(string text, int n = 5)
    {
        if (string.IsNullOrWhiteSpace(text))
            return (0, new List<(string, int)>());

        // Tokenize text into words
        List<string> words = Tokenize(text);

        // Generate N-grams
        List<string> nGrams = GenerateNGrams(words, n);

        // Count occurrences of each N-gram
        var nGramCounts = nGrams.GroupBy(ng => ng)
            .ToDictionary(g => g.Key, g => g.Count());

        int totalNGrams = nGrams.Count;
        int repeatedNGrams = nGramCounts.Values.Where(count => count > 2).Sum();

        // Extract and sort the repeating n-grams by descending count
        var repeatingPhrases = nGramCounts
            .Where(kvp => kvp.Value > 1)
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => (kvp.Key, kvp.Value))
            .ToList();

        double score = totalNGrams > 0 ? (double)repeatedNGrams / totalNGrams : 0;
        return (score, repeatingPhrases);
    }



    private static List<string> Tokenize(string text)
    {
        // Convert text to lowercase and split by non-alphanumeric characters
        return Regex.Split(text.ToLower(), @"\W+")
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .ToList();
    }

    private static List<string> GenerateNGrams(List<string> words, int n)
    {
        List<string> nGrams = new List<string>();

        for (int i = 0; i <= words.Count - n; i++)
        {
            string nGram = string.Join(" ", words.Skip(i).Take(n));
            nGrams.Add(nGram);
        }

        return nGrams;
    }
}
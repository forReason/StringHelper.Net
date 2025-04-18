using System.Text.RegularExpressions;

namespace StringHelper.Net.StringFunctionsNS;

public class CalculateTargetedNGramRepetitionScore
{
    /// <summary>
    /// Compares the n-grams in the target text against those in the history text and returns a repetition score.
    /// </summary>
    /// <param name="historyText">The larger reference corpus (e.g., past assistant messages)</param>
    /// <param name="targetText">The text to evaluate (e.g., the latest assistant message)</param>
    /// <param name="n">Size of the n-gram</param>
    /// <returns>A score between 0.0 and 1.0 indicating how repetitive the target text is</returns>
    public static (double Score, List<(string Phrase, int Count)> RepeatingPhrases) ComputeTargetedNGramRepetitionScore(string historyText, string targetText, int n = 5)
    {
        if (string.IsNullOrWhiteSpace(historyText) || string.IsNullOrWhiteSpace(targetText))
            return (0, new List<(string, int)>());

        List<string> historyWords = Tokenize(historyText);
        List<string> targetWords = Tokenize(targetText);

        HashSet<string> historyNGrams = GenerateNGrams(historyWords, n).ToHashSet();
        List<string> targetNGrams = GenerateNGrams(targetWords, n);

        Dictionary<string, int> matchCounts = new();

        foreach (string ngram in targetNGrams)
        {
            if (historyNGrams.Contains(ngram))
            {
                if (!matchCounts.ContainsKey(ngram))
                    matchCounts[ngram] = 0;
                matchCounts[ngram]++;
            }
        }

        int totalTargetNGrams = targetNGrams.Count;
        int repeatedCount = matchCounts.Values.Sum();

        var repeatingPhrases = matchCounts
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => (kvp.Key, kvp.Value))
            .ToList();

        double score = totalTargetNGrams > 0 ? (double)repeatedCount / totalTargetNGrams : 0;
        return (score, repeatingPhrases);
    }

    private static List<string> Tokenize(string text)
    {
        return Regex.Split(text.ToLower(), @"\W+")
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .ToList();
    }

    private static List<string> GenerateNGrams(List<string> words, int n)
    {
        List<string> nGrams = new();
        for (int i = 0; i <= words.Count - n; i++)
        {
            string nGram = string.Join(" ", words.Skip(i).Take(n));
            nGrams.Add(nGram);
        }
        return nGrams;
    }
}
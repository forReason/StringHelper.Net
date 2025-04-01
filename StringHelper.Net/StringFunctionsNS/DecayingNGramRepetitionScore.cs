using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;

namespace StringHelper.Net.StringFunctionsNS;

public class DecayingNGramRepetitionScore
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
    public static KeyValuePair<string, double>[] ComputeDecayingNGramRepetitionScore(string text, int n = 5, double recentRepetitionTreshold = 2, bool onlyReturnFlagged = false)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new KeyValuePair<string, double>[] {};
        
        List<string> words = Tokenize(text);
        List<string> nGrams = GenerateNGrams(words, n);
        
        int referenceWindow = 1000;
        double baseRate1WordNgramPer1000 = 50;
        double actualBaseRatePer1000 = baseRate1WordNgramPer1000 / (Math.Pow(2.5, n));
        double decayPerNgram = actualBaseRatePer1000 / (referenceWindow);

        // This dictionary tracks: (lastOccurrence, currentScore)
        Dictionary<string, (int LastOccurrence, double CurrentScore)> phrases 
            = new Dictionary<string, (int LastOccurrence, double CurrentScore)>();

        int wordIndex = 0;  // We'll move forward by 1 for each n-gram
        foreach (string ngram in nGrams)
        {
            if (phrases.TryGetValue(ngram, out var data))
            {
                int lastPos    = data.LastOccurrence;
                double oldScore= data.CurrentScore;

                // how many words since last occurrence?
                int delta = wordIndex - lastPos;

                // dynamic decay factor:
                double decay = Math.Exp(-decayPerNgram * delta);

                // new score = decayed old score + 1
                double newScore = oldScore * decay + 1.0;

                phrases[ngram] = (wordIndex, newScore);
            }
            else
            {
                phrases[ngram] = (wordIndex, 1.0);
            }

            wordIndex += 1; // move to next possible overlap
        }

        // Final pass: decay from last occurrence to the "end" (wordIndex)
        foreach (var key in phrases.Keys.ToList())
        {
            var (lastPos, oldScore) = phrases[key];
            int delta = wordIndex - lastPos; // how long since last occurrence
            double finalDecay = Math.Exp(-decayPerNgram * delta);
            double finalScore = oldScore * finalDecay;
            phrases[key] = (lastPos, finalScore);
        }
        
        // Build final dictionary of { nGram -> finalDecayedScore }
        var allScores = phrases.ToDictionary(
            p => p.Key, p => p.Value.CurrentScore);

        if (!onlyReturnFlagged)
        {
            // If you want a sorted result (descending by score), you can use LINQ OrderByDescending:
            var allScoresSorted = allScores
                .OrderByDescending(kvp => kvp.Value)
                .ToArray();

            // Return the sorted dictionary (though keep in mind that once it's a Dictionary,
            // you lose ordering again, so a list might be more appropriate if you strictly need “sorted”).
            return allScoresSorted;
        }

        // return flagged only if requested
        var flagged = new Dictionary<string, double>();
        foreach (var kvp in allScores)
        {
            if (kvp.Value > recentRepetitionTreshold)
            {
                flagged[kvp.Key] = kvp.Value;
            }
        }
        
        var flaggedSorted = flagged
            .OrderByDescending(kvp => kvp.Value)
            .ToArray();

        return flaggedSorted;
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
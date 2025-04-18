namespace StringHelper.Net.StringFunctionsNS;

public class MergeSequentialOverlappingNGrams
{
    public static List<KeyValuePair<string, double>> Merge(
        KeyValuePair<string, double>[] scoredNGrams)
    {
        if (scoredNGrams.Length == 0)
            return new List<KeyValuePair<string, double>>();

        // Infer n from the first n-gram
        int n = scoredNGrams[0].Key.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        // Map prefix to list of (phrase, score)
        var prefixMap = new Dictionary<string, List<(string Phrase, double Score)>>();

        foreach (var kv in scoredNGrams)
        {
            var words = kv.Key.Split(' ');
            var prefix = string.Join(" ", words.Take(n - 1));

            if (!prefixMap.ContainsKey(prefix))
                prefixMap[prefix] = new List<(string, double)>();

            prefixMap[prefix].Add((kv.Key, kv.Value));
        }

        var used = new HashSet<string>();
        var merged = new List<KeyValuePair<string, double>>();

        foreach (var kv in scoredNGrams)
        {
            if (used.Contains(kv.Key))
                continue;

            var chain = new List<(string Phrase, double Score)> { (kv.Key, kv.Value) };
            used.Add(kv.Key);
            var current = kv.Key;

            while (true)
            {
                var words = current.Split(' ');
                var suffix = string.Join(" ", words.Skip(1)); // last n-1 words

                if (prefixMap.TryGetValue(suffix, out var candidates))
                {
                    var next = candidates.FirstOrDefault(c => !used.Contains(c.Phrase));
                    if (next.Phrase != null)
                    {
                        chain.Add(next);
                        used.Add(next.Phrase);
                        current = next.Phrase;
                        continue;
                    }
                }

                break;
            }

            // Merge words into one long phrase
            var mergedWords = chain[0].Phrase.Split(' ').ToList();
            for (int i = 1; i < chain.Count; i++)
            {
                var nextWords = chain[i].Phrase.Split(' ');
                mergedWords.Add(nextWords.Last()); // Add only the last word
            }

            var mergedPhrase = string.Join(" ", mergedWords);
            var averageScore = chain.Select(x => x.Score).Average();

            merged.Add(new KeyValuePair<string, double>(mergedPhrase, averageScore));
        }

        return merged;
    }
}
namespace StringHelper.Net.StringFunctionsNS;

public class CalculateLevenshteinDistance
{
    /// <summary>
    /// Calculates the Levenshtein Distance between two strings.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="target">The target string.</param>
    /// <returns>The Levenshtein Distance between the strings.</returns>
    public static int LevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source)) return target?.Length ?? 0;
        if (string.IsNullOrEmpty(target)) return source.Length;

        var d = new int[source.Length + 1, target.Length + 1];

        for (int i = 0; i <= source.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= target.Length; j++) d[0, j] = j;

        for (int i = 1; i <= source.Length; i++)
        {
            for (int j = 1; j <= target.Length; j++)
            {
                int cost = source[i - 1] == target[j - 1] ? 0 : 1;

                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }

        return d[source.Length, target.Length];
    }
}
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace StringHelper.Net.StringFunctionsNS;

/// <summary>
/// Calculates a repetition score for progressing n-grams in a text stream,
/// allowing detection of repeated patterns and content duplication on the fly.
/// </summary>
public class ProgressingNGramRepetitionScore
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressingNGramRepetitionScore"/> class.
    /// </summary>
    /// <param name="ngramLength">The length of n-grams to track (default is 4).</param>
    public ProgressingNGramRepetitionScore(int ngramLength = 4)
    {
        NgramLength = ngramLength;
    }
    /// <summary>
    /// Clears all internal state and resets the tracker.
    /// </summary>
    public void Clear()
    {
        _Phrases.Clear();
        WordIndex = 0;
        _PhraseBuilder.Clear();
        _Words.Clear();
    }
    /// <summary>
    /// Gets or sets the number of words to use in an n-gram.
    /// </summary>
    public int NgramLength { get; set; }
    private Dictionary<string, (int LastOccurrence, double CurrentScore)> _Phrases
        = new Dictionary<string, (int LastOccurrence, double CurrentScore)>();
    /// <summary>
    /// Gets the current n-gram dictionary and scores, as a read-only dictionary.
    /// </summary>
    public ReadOnlyDictionary<string, (int LastOccurrence, double CurrentScore)> Phrases
        => new ReadOnlyDictionary<string, (int LastOccurrence, double CurrentScore)>(_Phrases);
    /// <summary>
    /// Gets the current word index used during scoring.
    /// </summary>
    public int WordIndex { get; private set; } = 0;
    private bool _WordlistChanged = false;
    private StringBuilder _PhraseBuilder = new StringBuilder();
    private Queue<string> _Words = new Queue<string>();
    private const int ReferenceWindow = 1000;
    private const double BaseRate1WordNgramPer1000 = 50;
    /// <summary>
    /// Returns a list of all n-gram scores, optionally filtering for only high-repetition phrases.
    /// </summary>
    /// <param name="onlyReturnFlagged">If true, returns only n-grams whose scores exceed the repetition threshold.</param>
    /// <param name="repetitionTreshold">The threshold above which an n-gram is considered a repetition (default is 1.5).</param>
    /// <returns>A sorted list of n-grams and their scores in descending order of score.</returns>
    public void AddTokens(string tokenChunk)
    {
        _PhraseBuilder.Append(tokenChunk);
        double actualBaseRatePer1000 = BaseRate1WordNgramPer1000 / (Math.Pow(2.5, NgramLength));
        double decayPerNgram = actualBaseRatePer1000 / (ReferenceWindow);
        if (ContainsWordBoundary(tokenChunk))
        {
            string chunk = _PhraseBuilder.ToString();
            _PhraseBuilder.Clear();
            List<string> words = Tokenize(chunk);
            if (!EndsWithWordBoundary(tokenChunk))
            {
                _PhraseBuilder.Append(words[^1]);
                words.RemoveAt(words.Count - 1);
            }

            foreach (string word in words)
            {
                _Words.Enqueue(word);
                // process NGram
                if (_Words.Count >= NgramLength)
                {
                    string nGram = string.Join(' ', _Words);
                    _Words.Dequeue();
                    if (_Phrases.TryGetValue(nGram, out var data))
                    {
                        int lastPos    = data.LastOccurrence;
                        double oldScore= data.CurrentScore;

                        // how many words since last occurrence?
                        int delta = WordIndex - lastPos;

                        // dynamic decay factor:
                        double decay = Math.Exp(-decayPerNgram * delta);

                        // new score = decayed old score + 1
                        double newScore = oldScore * decay + 1.0;

                        _Phrases[nGram] = (WordIndex, newScore);
                    }
                    else
                    {
                        _Phrases[nGram] = (WordIndex, 1.0);
                    }
                    _WordlistChanged = true;
                    WordIndex += 1;
                }
            }
        }

    }
    /// <summary>
    /// Returns a list of all n-gram scores, optionally filtering for only high-repetition phrases.
    /// </summary>
    /// <remarks>
    /// 0-1 - no repetition<br/>
    /// >1 some repetition
    /// >=2 Lots of repetition</remarks>
    /// <param name="onlyReturnFlagged">If true, returns only n-grams whose scores exceed the repetition threshold.</param>
    /// <param name="repetitionTreshold">The threshold above which an n-gram is considered a repetition (default is 1.5).</param>
    /// <returns>A sorted list of n-grams and their scores in descending order of score.</returns>
    public List<KeyValuePair<string, double>> GetAllScores(
        bool onlyReturnFlagged = false, double repetitionTreshold = 1.5)
    {
        // Final pass: decay from last occurrence to the "end" (wordIndex)
        double actualBaseRatePer1000 = BaseRate1WordNgramPer1000 / (Math.Pow(2.5, NgramLength));
        double decayPerNgram = actualBaseRatePer1000 / (ReferenceWindow);
        if (_WordlistChanged)
        {
            foreach (var key in _Phrases.Keys.ToList())
            {
                var (lastPos, oldScore) = _Phrases[key];
                if (lastPos >= WordIndex)
                    continue;
                int delta = WordIndex - lastPos; // how long since last occurrence
                double finalDecay = Math.Exp(-decayPerNgram * delta);
                double finalScore = oldScore * finalDecay;
                _Phrases[key] = (WordIndex, finalScore);
            }
            _WordlistChanged = false;
        }

        // Build final dictionary of { nGram -> finalDecayedScore }
        List<KeyValuePair<string, double>> allScoresSorted = _Phrases
            .OrderByDescending(kv => kv.Value.CurrentScore)
            .Select(kv => new KeyValuePair<string, double>(kv.Key, kv.Value.CurrentScore))
            .ToList();

        if (!onlyReturnFlagged)
        {
            return allScoresSorted;
        }

        // return flagged only if requested
        var flagged = new List<KeyValuePair<string, double>>();
        foreach (var kvp in allScoresSorted)
        {
            if (kvp.Value > repetitionTreshold)
            {
                flagged.Add(kvp);
            }
            else break;
        }

        return flagged;
    }

    /// <summary>
    /// Determines whether a given string contains any word boundaries (punctuation or whitespace).
    /// </summary>
    /// <param name="text">The text to evaluate.</param>
    /// <returns>True if a word boundary is detected; otherwise, false.</returns>
    private bool ContainsWordBoundary(string text)
    {
        return Regex.IsMatch(text, @"[\s\.,;:\?!\)\]]");
    }
    /// <summary>
    /// Determines whether the given text ends with a word boundary (e.g., space, period).
    /// </summary>
    /// <param name="text">The text to evaluate.</param>
    /// <returns>True if the text ends with a word boundary; otherwise, false.</returns>
    private bool EndsWithWordBoundary(string text)
    {
        return Regex.IsMatch(text, @"[\s\.,;:\?!\)\]]$");
    }
    /// <summary>
    /// Tokenizes the input text by converting it to lowercase and splitting by non-word characters.
    /// </summary>
    /// <param name="text">The text to tokenize.</param>
    /// <returns>A list of lowercase word tokens.</returns>
    private static List<string> Tokenize(string text)
    {
        // Convert text to lowercase and split by non-alphanumeric characters
        return Regex.Split(text.ToLower(), @"\W+")
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .ToList();
    }
}
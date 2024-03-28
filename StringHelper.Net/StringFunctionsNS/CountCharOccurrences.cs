using System.Text;

namespace StringHelper.Net.StringFunctionsNS;

/// <summary>
/// the purpose of this class is to count the occurrences of each character within a string.
/// </summary>
/// <remarks>
/// please note that this class is thread safe.
/// However, due to the locking nature, it is highly recommended to create one instance per worker thread
/// </remarks>
public class CountCharOccurrences
{
    private readonly Dictionary<char, int> _charOccurrences = new();
    private readonly object _isEvaluating = new ();
    private short _lastCheckedChar = -1;
    private int _lastCheckedOccurrenceCount = -1;

    /// <summary>
    /// renders the output to String like a3b2c4d1
    /// </summary>
    /// <remarks>
    /// - <see cref="Evaluate"/> must be executed first!<br/>
    /// - might yield weird results when numbers are included in the input string<br/>
    /// - The Output is NOT sorted!
    /// </remarks>
    /// <returns>String like a3b2c4d1</returns>
    public override string ToString()
    {
        lock (_isEvaluating)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<char, int> keyValue in _charOccurrences){
                builder.Append(keyValue.Key);
                builder.Append(keyValue.Value);
            }

            return builder.ToString();
        }
    }

    /// <summary>
    /// counts the occurrences for each char and returns a dictionary with each unique char and the respective count
    /// </summary>
    /// <remarks>
    /// - output can be sorted automatically <br/>
    /// - yields incorrect results if a character occurs more than int.Max times
    /// </remarks>
    /// <param name="input">the string to count the chars for</param>
    /// <param name="sortOption">specifies how the output is sorted</param>
    /// <returns>a dictionary which contains each unique char with the respective count</returns>
    public Dictionary<char, int> Evaluate(string input, SortOption sortOption)
    {
        // precondition checks
        if (string.IsNullOrEmpty(input)) return new Dictionary<char, int>();
        lock (_isEvaluating)
        {
            _charOccurrences.Clear();
            // count letters
            foreach (char c in input)
            {
                if (!_charOccurrences.TryAdd(c, 1))
                    _charOccurrences[c]++;
            }

            // check sorting (after building dict for less occurrences)
            switch (sortOption)
            {
                case SortOption.AlphabeticallyAscending:
                    _lastCheckedChar = -1;
                    foreach (char c in _charOccurrences.Values)
                    {
                        if (c < _lastCheckedChar)
                            return new Dictionary<char, int>(_charOccurrences.OrderBy(entry => entry.Key));
                        _lastCheckedChar = (short)c;
                    }

                    break;
                case SortOption.AlphabeticallyDescending:
                    _lastCheckedChar = short.MaxValue;
                    foreach (char c in _charOccurrences.Values)
                    {
                        if (c > _lastCheckedChar)
                            return new Dictionary<char, int>(_charOccurrences.OrderByDescending(entry => entry.Key));
                        _lastCheckedChar = (short)c;
                    }

                    break;
                case SortOption.CharOccurrencesAscending:
                    _lastCheckedOccurrenceCount = -1;
                    foreach (int key in _charOccurrences.Values)
                    {
                        if (key < _lastCheckedOccurrenceCount)
                            return new Dictionary<char, int>(_charOccurrences.OrderBy(entry => entry.Value));
                        _lastCheckedOccurrenceCount = key;
                    }

                    break;
                case SortOption.CharOccurrencesDescending:
                    _lastCheckedOccurrenceCount = int.MaxValue;
                    foreach (int key in _charOccurrences.Values)
                    {
                        if (key > _lastCheckedOccurrenceCount)
                            return new Dictionary<char, int>(_charOccurrences.OrderByDescending(entry => entry.Value));
                        _lastCheckedOccurrenceCount = key;
                    }

                    break;
                case SortOption.None:
                    break;
            }

            // return a new instance of the sorted dictionary to avoid reference modifications
            return new Dictionary<char, int>(_charOccurrences);
        }
    }

    /// <summary>
    /// Provides sorting options for <see cref="Evaluate"/>
    /// </summary>
    public enum SortOption
    {
        /// <summary>
        /// no sorting, same order than the occurrences in the input string
        /// </summary>
        None,
        /// <summary>
        /// sort alphabetically (0 1 2 a b c)
        /// </summary>
        AlphabeticallyAscending,
        /// <summary>
        /// sort alphabetically descending (z y x 2 1 0)
        /// </summary>
        AlphabeticallyDescending,
        /// <summary>
        /// sort the characters which appeared most often first
        /// </summary>
        CharOccurrencesAscending,
        /// <summary>
        /// sort the characters which appeared the least first
        /// </summary>
        CharOccurrencesDescending
    }
}
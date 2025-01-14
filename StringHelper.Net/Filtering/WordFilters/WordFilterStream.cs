using System.Text;

namespace StringHelper.Net.Filtering.WordFilters;

/// <summary>
/// Wrapper function for WordFilter which allows to process a string in chunks as its being generated.
/// </summary>
/// <remarks>
/// no multithreading. only one stream can be checked at a time.<br/>
/// Clear() before each new string
/// </remarks>
public class WordFilterStream
{
    /// <summary>
    /// the internal filter
    /// </summary>
    private readonly WordFilter _Funnel;
    /// <summary>
    /// the current filter nodes
    /// </summary>
    private readonly List<WordFilterNode> _CurrentNodes = new List<WordFilterNode>();
    /// <summary>
    /// buffer for processing
    /// </summary>
    private readonly StringBuilder _Buffer = new StringBuilder();
    /// <summary>
    /// creates a new word-filter with the given input filters.
    /// </summary>
    /// <remarks>
    /// Filters need to be loaded, which is happening multithreaded
    /// </remarks>
    /// <param name="inputFilters">the filters to load</param>
    public WordFilterStream(string[] inputFilters)
    {
        _Funnel = new WordFilter(inputFilters);
        Clear(); // Initialize the state
    }

    /// <summary>
    /// adds a new, additional filter. This can be done while matching, but won't necessarily match on the current iteration
    /// </summary>
    /// <param name="filter">the match filter to add</param>
    public void AddFilter(string filter) => _Funnel.AddFilter(filter);

    /// <summary>
    /// Processes a chunk of text and checks for matches.
    /// </summary>
    /// <param name="chunk">The text chunk to process.</param>
    /// <param name="matchType">The type of match to perform (Contains or BeginsWith).</param>
    /// <returns>True if a match is found; otherwise, false.</returns>
    public bool Process(string chunk, MatchType matchType = MatchType.Contains)
    {
        foreach (char c in chunk)
        {
            if (_Buffer.Length > 0 && WordFilter.IsDelimiter(c))
            {
                string word = _Buffer.ToString();
                _Buffer.Clear();

                if (_Funnel.ProcessWord(word, _CurrentNodes, matchType))
                    return true;
                if (!_CurrentNodes.Any())
                    return false;

                continue;
            }

            if (char.IsLetter(c)) _Buffer.Append(char.ToUpperInvariant(c));
        }

        // Process remaining buffer at the end
        if (_Buffer.Length > 0)
        {
            string word = _Buffer.ToString();
            if (_Funnel.ProcessWord(word, _CurrentNodes, matchType)) return true;
        }

        return false;
    }

    /// <summary>
    /// Resets the internal state to allow a new stream to be processed.
    /// </summary>
    public void Clear()
    {
        _CurrentNodes.Clear();
        _CurrentNodes.Add(_Funnel.RootNode);
        _Buffer.Clear();
    }
}
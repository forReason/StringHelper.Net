using System.Text;

namespace StringHelper.Net.Filtering.WordFilters;

/// <summary>
/// A string funnel is kind of like a tree structure for efficiently Filtering "BeginsWith" and "Contains" on string arrays.<br/>
/// This is used to check if sentences contain specific word sequences. eg: "I can't do that"
/// </summary>
/// <remarks>This is a forward-feeding funnel. Thus, "EndsWith" is not implemented.</remarks>
public class WordFilter
{
    /// <summary>
    /// threading control
    /// </summary>
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

    /// <summary>
    /// creates a new word-filter with the given input filters.
    /// </summary>
    /// <remarks>
    /// Filters need to be loaded, which is happening multithreaded
    /// </remarks>
    /// <param name="inputFilters">the filters to load</param>
    public WordFilter(string[] inputFilters)
    {
        RootNode = new WordFilterNode("ROOT");
        Parallel.ForEach(inputFilters, AddFilter);
    }

    /// <summary>
    /// checks an entire string for any match in an efficient manner
    /// </summary>
    /// <param name="input">the string to match against the filter. can be run in parallel</param>
    /// <param name="matchType">begins with or contains (contains includes begins :)</param>
    /// <returns>true when a match is found</returns>
    public bool Match(string input, MatchType matchType = MatchType.Contains)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        StringBuilder sb = new StringBuilder();
        List<WordFilterNode> currentNodes = new List<WordFilterNode> { RootNode };

        foreach (char c in input.AsSpan())
        {
            if (sb.Length > 0 && IsDelimiter(c))
            {
                string word = sb.ToString();
                sb.Clear();

                if (ProcessWord(word, currentNodes, matchType))
                    return true;
                if (!currentNodes.Any())
                    return false;

                continue;
            }

            if (char.IsLetter(c)) sb.Append(char.ToUpperInvariant(c));
        }

        // Process the last word in the input
        if (sb.Length > 0)
        {
            string word = sb.ToString();
            if (ProcessWord(word, currentNodes, matchType)) return true;
        }

        return false;
    }

    /// <summary>
    /// adds a new, additional filter. This can be done while matching, but won't necessarily match on the current iteration
    /// </summary>
    /// <param name="filter">the match filter to add</param>
    public void AddFilter(string filter)
    {
        string[] tokens = ChunkStringToWords(filter.ToUpperInvariant());

        _lock.EnterWriteLock();
        try
        {
            WordFilterNode selectedNode = RootNode;
            foreach (string token in tokens)
            {
                if (!selectedNode.Children.ContainsKey(token))
                {
                    selectedNode.Children[token] = new WordFilterNode(token);
                }
                selectedNode = selectedNode.Children[token];
            }
            selectedNode.IsFinal = true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Processes the next word in the chain. this basically contains all the comparison logic
    /// </summary>
    /// <param name="word">the word to match</param>
    /// <param name="currentNodes">the currently selected filters</param>
    /// <param name="matchType">begins or contains</param>
    /// <returns></returns>
    internal bool ProcessWord(string word, List<WordFilterNode> currentNodes, MatchType matchType)
    {
        for (int i = currentNodes.Count - 1; i >= 0; i--)
        {
            WordFilterNode currentNode = currentNodes[i];
            currentNodes.RemoveAt(i);

            if (currentNode.Children.ContainsKey(word))
            {
                currentNode = currentNode.Children[word];
                if (currentNode.IsFinal) return true;
                currentNodes.Add(currentNode);
            }

            if (matchType == MatchType.Contains && RootNode.Children.ContainsKey(word))
            {
                currentNodes.Add(RootNode.Children[word]);
            }
        }
        if (matchType == MatchType.Contains)
            currentNodes.Add(RootNode);

        return false;
    }

    /// <summary>
    /// the root node contains all nodes for the tree
    /// </summary>
    public WordFilterNode RootNode { get; private set; }

    /// <summary>
    /// function which defines where words get split
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    internal static bool IsDelimiter(char c) =>
        char.IsWhiteSpace(c) // spaces
        || (char.IsPunctuation(c) && c != '\'') // punctuation chars, except apostrophe (can't)
        || char.IsSeparator(c); // separators (||)

    /// <summary>
    /// internal function to separate each word (e.g. I will go ["I", "will", "go"]
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string[] ChunkStringToWords(string input)
    {
        List<string> words = new List<string>();
        StringBuilder sb = new StringBuilder();

        foreach (char c in input.AsSpan())
        {
            if (sb.Length > 0 && IsDelimiter(c))
            {
                words.Add(sb.ToString());
                sb.Clear();
                continue;
            }
            if (char.IsLetter(c)) sb.Append(char.ToUpperInvariant(c));
        }

        if (sb.Length > 0)
        {
            words.Add(sb.ToString());
            sb.Clear();
        }

        return words.ToArray();
    }
}
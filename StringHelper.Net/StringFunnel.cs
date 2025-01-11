using System.Text;

namespace StringHelper.Net;

public class StringFunnelStream
{
    private StringFunnel Funnel;
    private List<StringFunnelNode> CurrentNodes = new List<StringFunnelNode>();
    private StringBuilder Buffer = new StringBuilder();

    public StringFunnelStream(string[] filters)
    {
        Funnel = new StringFunnel(filters);
        Clear(); // Initialize the state
    }

    public void AddFilter(string filter) => Funnel.AddFilter(filter);

    /// <summary>
    /// Processes a chunk of text and checks for matches.
    /// </summary>
    /// <param name="chunk">The text chunk to process.</param>
    /// <param name="matchType">The type of match to perform (Contains or BeginsWith).</param>
    /// <returns>True if a match is found; otherwise, false.</returns>
    public bool Process(string chunk, StringFunnel.MatchType matchType = StringFunnel.MatchType.Contains)
    {
        Buffer.Append(chunk);
        foreach (char c in chunk)
        {
            if (Buffer.Length > 0 && StringFunnel.IsDelimiter(c))
            {
                string word = Buffer.ToString();
                Buffer.Clear();

                if (Funnel.ProcessWord(word, CurrentNodes, matchType)) return true;
                if (!CurrentNodes.Any()) return false;

                continue;
            }

            if (char.IsLetter(c)) Buffer.Append(char.ToUpperInvariant(c));
        }

        // Process remaining buffer at the end
        if (Buffer.Length > 0)
        {
            string word = Buffer.ToString();
            if (Funnel.ProcessWord(word, CurrentNodes, matchType)) return true;
        }

        return false;
    }

    /// <summary>
    /// Resets the internal state to allow a new stream to be processed.
    /// </summary>
    public void Clear()
    {
        CurrentNodes.Clear();
        CurrentNodes.Add(Funnel.RootNode);
        Buffer.Clear();
    }
}


/// <summary>
/// A string funnel is kind of like a tree structure for efficiently filtering "BeginsWith" and "Contains" on string arrays.
/// </summary>
/// <remarks>This is a forward-feeding funnel. Thus, "EndsWith" is not implemented.</remarks>
public class StringFunnel
{
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

    public StringFunnel(string[] inputFilters)
    {
        RootNode = new StringFunnelNode("ROOT");
        Parallel.ForEach(inputFilters, AddFilter);
    }

    public bool Match(string input, MatchType matchType = MatchType.Contains)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        StringBuilder sb = new StringBuilder();
        List<StringFunnelNode> currentNodes = new List<StringFunnelNode> { RootNode };

        foreach (char c in input.AsSpan())
        {
            if (sb.Length > 0 && IsDelimiter(c))
            {
                string word = sb.ToString();
                sb.Clear();

                if (ProcessWord(word, currentNodes, matchType)) return true;
                if (!currentNodes.Any()) return false;

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

    public void AddFilter(string filter)
    {
        string[] tokens = ChunkStringToWords(filter.ToUpperInvariant());

        _lock.EnterWriteLock();
        try
        {
            StringFunnelNode selectedNode = RootNode;
            foreach (string token in tokens)
            {
                if (!selectedNode.Children.ContainsKey(token))
                {
                    selectedNode.Children[token] = new StringFunnelNode(token);
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

    internal bool ProcessWord(string word, List<StringFunnelNode> currentNodes, MatchType matchType)
    {
        for (int i = currentNodes.Count - 1; i >= 0; i--)
        {
            StringFunnelNode currentNode = currentNodes[i];
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

        return false;
    }

    public StringFunnelNode RootNode { get; private set; }

    internal static bool IsDelimiter(char c) => char.IsWhiteSpace(c) || char.IsPunctuation(c) || char.IsSeparator(c);

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

    public enum MatchType
    {
        Contains,
        BeginsWith,
    }
}

public sealed class StringFunnelNode
{
    public StringFunnelNode(string word)
    {
        Word = word;
    }

    public string Word { get; set; }
    public object NodeLock { get; } = new object();
    public bool IsFinal { get; set; } = false;
    public Dictionary<string, StringFunnelNode> Children { get; } = new Dictionary<string, StringFunnelNode>();
}

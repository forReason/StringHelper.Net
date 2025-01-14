namespace StringHelper.Net.Filtering.WordFilters;

/// <summary>
/// a word-filter node contains exactly one word and children
/// </summary>
public sealed class WordFilterNode
{
    /// <summary>
    /// Creates the Filter Node with the word initialized
    /// </summary>
    /// <param name="word"></param>
    public WordFilterNode(string word)
    {
        Word = word;
    }

    /// <summary>
    /// the actual word of the Node
    /// </summary>
    public string Word { get; set; }
    /// <summary>
    /// Thread control
    /// </summary>
    public object NodeLock { get; } = new object();
    /// <summary>
    /// Marks this node as a final node (required for matches)
    /// </summary>
    public bool IsFinal { get; set; } = false;
    /// <summary>
    /// further nodes in the filter chain
    /// </summary>
    public Dictionary<string, WordFilterNode> Children { get; private set; } = new Dictionary<string, WordFilterNode>();
    /// <summary>
    /// returns the name and the children
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Word} - {Children.Count} children";
    }
}
namespace StringHelper.Net.Filtering.WordFilters;

/// <summary>
///  specifies the comparison strictness for a WordFilter
/// </summary>
public enum MatchType
{
    /// <summary>
    /// includes "BeginsWith" but needs to check the entire string
    /// </summary>
    Contains,
    /// <summary>
    /// only checks the start of a string and thus can be very fast
    /// </summary>
    BeginsWith,
}
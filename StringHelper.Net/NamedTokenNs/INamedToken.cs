namespace StringHelper.Net.NamedTokenNs;

public interface INamedToken : IEquatable<INamedToken>, IComparable<INamedToken>
{
    Guid Id { get; }
    string Name { get; }
}
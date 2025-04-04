namespace StringHelper.Net.NamedToken;

public interface INamedToken : IEquatable<INamedToken>, IComparable<INamedToken>
{
    Guid Id { get; }
    string Name { get; }
}
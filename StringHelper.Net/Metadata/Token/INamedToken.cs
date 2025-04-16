namespace StringHelper.Net.Metadata.Token;

public interface INamedToken : IEquatable<INamedToken>, IComparable<INamedToken>
{
    Guid Id { get; }
    string Name { get; }
}
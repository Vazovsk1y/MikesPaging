using MikesPaging.Core.Common.Enums;

namespace MikesPaging.Core.Common.Interfaces;

public interface IFilter
{

}
public interface IFilter<T> : IFilter 
    where T : Enum
{
    T FilterBy { get; }

    FilteringOperators Operator { get; }

    string Value { get; }
}
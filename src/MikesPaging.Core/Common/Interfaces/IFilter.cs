using MikesPaging.Core.Common.Enums;

namespace MikesPaging.Core.Common.Interfaces;

public interface IFilter
{

}
public interface IFilter<TFilterBy> : IFilter 
    where TFilterBy : Enum
{
    TFilterBy FilterBy { get; }

    FilteringOperators Operator { get; }

    string Value { get; }
}
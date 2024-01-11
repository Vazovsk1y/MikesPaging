using MikesPaging.Core.Common.Enums;

namespace MikesPaging.Core.Common.Interfaces;

public interface IFilteringOptions
{

}

public interface IFilteringOptions<T> : 
    IFilteringOptions where T : class, IFilter
{
    IReadOnlyCollection<T> Filters { get; }

    Logic Logic { get; }
}
namespace MikesPaging.Core.Common.Interfaces;

public interface IFilteredPage<TItem, TFiltering> : IPage<TItem>
    where TFiltering : class, IFilteringOptions
{
    TFiltering? AppliedFiltering { get; }
}

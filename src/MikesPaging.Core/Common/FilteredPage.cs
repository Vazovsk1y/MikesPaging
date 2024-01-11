using MikesPaging.Core.Common.Interfaces;

namespace MikesPaging.Core.Common;

public abstract record FilteredPage<TItem, TFiltering> : Page<TItem>, IFilteredPage<TItem, TFiltering>
    where TFiltering : class, IFilteringOptions
{
    public TFiltering? AppliedFiltering { get; }

    protected FilteredPage(
        IReadOnlyCollection<TItem> items,
        int totalItemsCount,
        TFiltering? filteringOptions,
        PagingOptions? pagingOptions = null)
        : base(items, totalItemsCount, pagingOptions)
    {
        AppliedFiltering = filteringOptions;
    }
}
using MikesPaging.Core.Common.Interfaces;

namespace MikesPaging.Core.Common;

public abstract record SortedPage<TItem, TSorting> : Page<TItem>, ISortedPage<TItem, TSorting>
    where TSorting : class, ISortingOptions
{
    public TSorting? AppliedSorting { get; }

    protected SortedPage(
        IReadOnlyCollection<TItem> items,
        int totalItemsCount,
        TSorting? sortingOptions,
        PagingOptions? pagingOptions = null)
        : base(items, totalItemsCount, pagingOptions)
    {
        AppliedSorting = sortingOptions;
    }
}
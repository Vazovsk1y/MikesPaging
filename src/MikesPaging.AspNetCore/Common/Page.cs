using MikesPaging.AspNetCore.Common.Interfaces;
using MikesPaging.AspNetCore.Exceptions;

namespace MikesPaging.AspNetCore.Common;
public abstract record Page<TItem> : IPage<TItem>
{
    private const int StartCountingFrom = 1;

    public IReadOnlyCollection<TItem> Items { get; }

    public int PageIndex { get; }

    public int TotalItemsCount { get; }

    public int TotalPagesCount { get; }

    public bool HasNextPage => PageIndex < TotalPagesCount;

    public bool HasPreviousPage => PageIndex > StartCountingFrom;

    protected Page(
        IReadOnlyCollection<TItem> items, 
        int totalItemsCount, 
        PagingOptions? pagingOptions)
    {
        PagingException.ThrowIf(pagingOptions is { PageIndex: <= 0 }, Errors.Paging.PageIndexMustBeGreaterThanZero);
        PagingException.ThrowIf(pagingOptions is { PageSize: <= 0 }, Errors.Paging.PageSizeMustBeGreaterThanZero);
        PagingException.ThrowIf(items.Count > totalItemsCount, Errors.Paging.TotalItemsCountCannotBeLowerThanCurrentItemsCount);
        PagingException.ThrowIf(pagingOptions is null && totalItemsCount != items.Count, Errors.Paging.TotalItemsCountMustBeEqualToCurrentItemsCount);

        Items = items;
        TotalItemsCount = totalItemsCount;
        PageIndex = pagingOptions?.PageIndex ?? StartCountingFrom;
        TotalPagesCount = pagingOptions is null || pagingOptions is not null && totalItemsCount <= pagingOptions.PageSize ?
            StartCountingFrom 
            : 
            (int)Math.Ceiling(totalItemsCount / (double)pagingOptions!.PageSize);
    }
}

public abstract record Page<TItem, TSorting, TFiltering> : 
    Page<TItem>
    where TSorting : class, ISortingOptions
    where TFiltering : class, IFilteringOptions
{
    public TFiltering? AppliedFiltering { get; }

    public TSorting? AppliedSorting { get; }

    protected Page(
        IReadOnlyCollection<TItem> items,
        int totalItemsCount,
        TSorting? sortingOptions,
        TFiltering? filteringOptions,
        PagingOptions? pagingOptions)
        : base(items, totalItemsCount, pagingOptions)
    {
        AppliedSorting = sortingOptions;
        AppliedFiltering = filteringOptions;
    }
}
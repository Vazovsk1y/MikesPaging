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

    protected Page(IReadOnlyCollection<TItem> items, int totalItemsCount, PagingOptions? pagingOptions = null)
    {
        PagingException.ThrowIf(pagingOptions is { PageIndex: <= 0 }, "Page index must be greater than zero.");
        PagingException.ThrowIf(pagingOptions is { PageSize: <= 0 }, "Page size must be greater than zero.");
        PagingException.ThrowIf(items.Count > totalItemsCount, "Total items count can't be lower than current items count.");
        PagingException.ThrowIf(pagingOptions is null && totalItemsCount != items.Count, "If paging options is not passed, total items count must be equal to current items count.");

        Items = items;
        TotalItemsCount = totalItemsCount;
        PageIndex = pagingOptions is null ? StartCountingFrom : pagingOptions.PageIndex;
        TotalPagesCount = CalculateTotalPages(pagingOptions, totalItemsCount);
    }

    private static int CalculateTotalPages(PagingOptions? pagingOptions, int totalItemsCount)
    {
        if (pagingOptions is null)
        {
            return StartCountingFrom;
        }

        return totalItemsCount < pagingOptions.PageSize ? StartCountingFrom : (int)Math.Ceiling(totalItemsCount / (double)pagingOptions.PageSize);
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
        PagingOptions? pagingOptions = null)
        : base(items, totalItemsCount, pagingOptions)
    {
        AppliedSorting = sortingOptions;
        AppliedFiltering = filteringOptions;
    }
}
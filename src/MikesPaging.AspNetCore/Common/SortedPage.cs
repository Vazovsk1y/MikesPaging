namespace MikesPaging.AspNetCore.Common;

/// <summary>
/// Represents a page of items in a paginated and sorted collection.
/// </summary>
/// <typeparam name="TItem">The type of the items in the page.</typeparam>
/// <typeparam name="TSorting">The type of sorting options applied to the collection.</typeparam>
public abstract record SortedPage<TItem, TSorting> : Page<TItem>
    where TSorting : class, ISortingOptions
{
    /// <summary>
    /// Gets the sorting options applied to the collection.
    /// </summary>
    public TSorting? AppliedSorting { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortedPage{TItem, TSorting}"/> class.
    /// </summary>
    /// <param name="items">The collection of items in the page.</param>
    /// <param name="totalItemsCount">The total number of items across all pages.</param>
    /// <param name="sortingOptions">The sorting options applied to the collection.</param>
    /// <param name="pagingOptions">The paging options for the page.</param>
    protected SortedPage(
        IReadOnlyCollection<TItem> items,
        int totalItemsCount,
        TSorting? sortingOptions,
        PagingOptions? pagingOptions)
        : base(items, totalItemsCount, pagingOptions)
    {
        AppliedSorting = sortingOptions;
    }
}
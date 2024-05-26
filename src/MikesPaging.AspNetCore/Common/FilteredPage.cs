namespace MikesPaging.AspNetCore.Common;

/// <summary>
/// Represents a page of items in a paginated and filtered collection.
/// </summary>
/// <typeparam name="TItem">The type of the items in the page.</typeparam>
/// <typeparam name="TFiltering">The type of filtering options applied to the collection.</typeparam>
public abstract record FilteredPage<TItem, TFiltering> : Page<TItem>
    where TFiltering : class, IFilteringOptions
{
    /// <summary>
    /// Gets the filtering options applied to the collection.
    /// </summary>
    public TFiltering? AppliedFiltering { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilteredPage{TItem, TFiltering}"/> class.
    /// </summary>
    /// <param name="items">The collection of items in the page.</param>
    /// <param name="totalItemsCount">The total number of items across all pages.</param>
    /// <param name="filteringOptions">The filtering options applied to the collection.</param>
    /// <param name="pagingOptions">The paging options for the page.</param>
    protected FilteredPage(
        IReadOnlyCollection<TItem> items,
        int totalItemsCount,
        TFiltering? filteringOptions,
        PagingOptions? pagingOptions)
        : base(items, totalItemsCount, pagingOptions)
    {
        AppliedFiltering = filteringOptions;
    }
}
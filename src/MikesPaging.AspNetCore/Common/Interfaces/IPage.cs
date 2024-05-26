namespace MikesPaging.AspNetCore.Common.Interfaces;

/// <summary>
/// Represents a page of items in a paginated collection.
/// </summary>
/// <typeparam name="TItem">The type of the items in the page.</typeparam>
public interface IPage<out TItem>
{
    /// <summary>
    /// Gets a value indicating whether there is a next page available.
    /// </summary>
    /// <value>
    /// <c>true</c> if there is a next page; otherwise, <c>false</c>.
    /// </value>
    bool HasNextPage { get; }

    /// <summary>
    /// Gets a value indicating whether there is a previous page available.
    /// </summary>
    /// <value>
    /// <c>true</c> if there is a previous page; otherwise, <c>false</c>.
    /// </value>
    bool HasPreviousPage { get; }

    /// <summary>
    /// Gets the collection of items on the current page.
    /// </summary>
    /// <value>
    /// A read-only collection of items of type <typeparamref name="TItem"/>.
    /// </value>
    IReadOnlyCollection<TItem> Items { get; }

    /// <summary>
    /// Gets the one-based index of the current page.
    /// </summary>
    /// <value>
    /// The index of the current page.
    /// </value>
    int PageIndex { get; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    /// <value>
    /// The total number of items.
    /// </value>
    int TotalItemsCount { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    /// <value>
    /// The total number of pages.
    /// </value>
    int TotalPagesCount { get; }
}

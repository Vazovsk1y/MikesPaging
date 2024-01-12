namespace MikesPaging.AspNetCore.Common.Interfaces;

public interface IPage<TItem>
{
    bool HasNextPage { get; }
    bool HasPreviousPage { get; }
    IReadOnlyCollection<TItem> Items { get; }
    int PageIndex { get; }
    int TotalItemsCount { get; }
    int TotalPagesCount { get; }
}
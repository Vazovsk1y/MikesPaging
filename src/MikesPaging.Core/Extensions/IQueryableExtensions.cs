using MikesPaging.Core.Common;

namespace MikesPaging.Core.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> collection, PagingOptions? pagingOptions)
    {
        if (pagingOptions is null)
        {
            return collection;
        }

        return collection
            .Skip((pagingOptions.PageIndex - 1) * pagingOptions.PageSize)
            .Take(pagingOptions.PageSize);
    }
}

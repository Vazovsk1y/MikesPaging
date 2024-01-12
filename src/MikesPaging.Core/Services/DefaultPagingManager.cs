using MikesPaging.Core.Common;
using MikesPaging.Core.Exceptions;
using MikesPaging.Core.Services.Interfaces;

namespace MikesPaging.Core.Services;

public class DefaultPagingManager<TSource> : IPagingManager<TSource>
{
    public IQueryable<TSource> ApplyPaging(IQueryable<TSource> source, PagingOptions? pagingOptions)
    {
        if (pagingOptions is null)
        {
            return source;
        }

        PagingException.ThrowIf(pagingOptions is { PageIndex: <= 0 }, "Page index must be greater than zero.");
        PagingException.ThrowIf(pagingOptions is { PageSize: <= 0 }, "Page size must be greater than zero.");

        return source
           .Skip((pagingOptions.PageIndex - 1) * pagingOptions.PageSize)
           .Take(pagingOptions.PageSize);
    }
}
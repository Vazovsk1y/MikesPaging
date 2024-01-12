using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Exceptions;
using MikesPaging.AspNetCore.Services.Interfaces;

namespace MikesPaging.AspNetCore.Services;

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
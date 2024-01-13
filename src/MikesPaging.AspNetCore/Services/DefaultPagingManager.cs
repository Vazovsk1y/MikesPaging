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

        PagingException.ThrowIf(pagingOptions is { PageIndex: <= 0 }, Errors.Paging.PageIndexMustBeGreaterThanZero);
        PagingException.ThrowIf(pagingOptions is { PageSize: <= 0 }, Errors.Paging.PageSizeMustBeGreaterThanZero);

        return source
           .Skip((pagingOptions.PageIndex - 1) * pagingOptions.PageSize)
           .Take(pagingOptions.PageSize);
    }
}
using MikesPaging.AspNetCore.Common;

namespace MikesPaging.AspNetCore.Services.Interfaces;

public interface IPagingManager<TSource>
{
    IQueryable<TSource> ApplyPaging(IQueryable<TSource> source, PagingOptions? pagingOptions);
}
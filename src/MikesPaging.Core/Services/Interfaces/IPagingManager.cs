using MikesPaging.Core.Common;

namespace MikesPaging.Core.Services.Interfaces;

public interface IPagingManager<TSource>
{
    IQueryable<TSource> ApplyPaging(IQueryable<TSource> source, PagingOptions? pagingOptions);
}
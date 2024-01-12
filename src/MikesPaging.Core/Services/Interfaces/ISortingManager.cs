using MikesPaging.Core.Common;

namespace MikesPaging.Core.Services.Interfaces;

public interface ISortingManager<TSource>
{
    IQueryable<TSource> ApplySorting<TSortBy>(IQueryable<TSource> source, SortingOptions<TSortBy>? sortingOptions)
        where TSortBy : Enum;
}
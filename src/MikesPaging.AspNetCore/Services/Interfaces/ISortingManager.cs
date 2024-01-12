using MikesPaging.AspNetCore.Common;

namespace MikesPaging.AspNetCore.Services.Interfaces;

public interface ISortingManager<TSource>
{
    IQueryable<TSource> ApplySorting<TSortBy>(IQueryable<TSource> source, SortingOptions<TSortBy>? sortingOptions)
        where TSortBy : Enum;
}
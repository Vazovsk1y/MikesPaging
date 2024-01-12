using MikesPaging.Core.Common.Interfaces;

namespace MikesPaging.Core.Services.Interfaces;

public interface ISortingManager<TSource>
{
    IQueryable<TSource> ApplySorting<TSortBy>(IQueryable<TSource> source, ISortingOptions<TSortBy>? sortingOptions)
        where TSortBy : Enum;
}
using MikesPaging.Core.Common.Interfaces;

namespace MikesPaging.Core.Services.Interfaces;

public interface ISortingManager<TSource>
{
    IQueryable<TSource> ApplySorting<TSorting, T>(IQueryable<TSource> source, TSorting? sortingOptions)
        where TSorting : class, ISortingOptions<T>
        where T : Enum;
}

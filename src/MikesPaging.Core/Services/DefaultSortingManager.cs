using Microsoft.Extensions.DependencyInjection;
using MikesPaging.Core.Common.Enums;
using MikesPaging.Core.Common.Interfaces;
using MikesPaging.Core.Exceptions;
using MikesPaging.Core.Services.Interfaces;
using System.Linq.Expressions;

namespace MikesPaging.Core.Services;

public class DefaultSortingManager<TSource>(IServiceScopeFactory serviceScopeFactory) : ISortingManager<TSource>
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    public IQueryable<TSource> ApplySorting<TSorting, T>(IQueryable<TSource> source, TSorting? sortingOptions)
        where TSorting : class, ISortingOptions<T>
        where T : Enum
    {
        if (sortingOptions is null)
        {
            return source;
        }

        MikesPagingException.ThrowIf(sortingOptions.SortDirection == SortDirections.Unknown, "Undefined sort direction.");

        using var scope = _serviceScopeFactory.CreateScope();
        var configurationType = typeof(ISortingConfiguration< , >).MakeGenericType(typeof(TSource), typeof(T));
        var configuration = scope.ServiceProvider.GetService(configurationType);
        if (configuration is ISortingConfiguration<TSource, T> castedConfiguration)
        {
            if (castedConfiguration.Sorters.TryGetValue(sortingOptions.SortBy, out Expression<Func<TSource, object>>? configuredSorter))
            {
                return Sort(source, sortingOptions.SortDirection, configuredSorter);
            }
        }

        try
        {
            var parameter = Expression.Parameter(typeof(TSource), "x");
            var property = Expression.Property(parameter, sortingOptions.SortBy.ToString());

            var lambda = Expression.Lambda<Func<TSource, object>>(property, parameter);

            return Sort(source, sortingOptions.SortDirection, lambda);
        }
        catch (Exception ex)
        {
            throw new MikesPagingException(ex.Message);
        }
    }

    private static IOrderedQueryable<TSource> Sort<TBy>(IQueryable<TSource> collection, SortDirections sortDirection, Expression<Func<TSource, TBy>> expression)
    {
        return sortDirection == SortDirections.Ascending ? collection.OrderBy(expression) : collection.OrderByDescending(expression);
    }
}

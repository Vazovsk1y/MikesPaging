using Microsoft.Extensions.DependencyInjection;
using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Common.Interfaces;
using MikesPaging.AspNetCore.Exceptions;
using MikesPaging.AspNetCore.Services.Interfaces;
using System.Linq.Expressions;

namespace MikesPaging.AspNetCore.Services;

public class DefaultSortingManager<TSource>(IServiceScopeFactory serviceScopeFactory) : ISortingManager<TSource>
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    public IQueryable<TSource> ApplySorting<TSortBy>(IQueryable<TSource> source, SortingOptions<TSortBy>? sortingOptions)
        where TSortBy : MikesPagingEnum
    {
        if (sortingOptions is null)
        {
            return source;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var configurationType = typeof(ISortingConfiguration< , >).MakeGenericType(typeof(TSource), typeof(TSortBy));
        var configuration = scope.ServiceProvider.GetService(configurationType);

        try
        {
            if (configuration is ISortingConfiguration<TSource, TSortBy> castedConfiguration)
            {
                if (castedConfiguration.Sorters.TryGetValue(sortingOptions.SortBy, out var configuredSorter))
                {
                    return Sort(source, sortingOptions.SortDirection, configuredSorter);
                }
            }

            var parameter = Expression.Parameter(typeof(TSource), "x");
            var property = Expression.Property(parameter, sortingOptions.SortBy.PropertyName);
            var conversion = Expression.Convert(property, typeof(object));

            var lambda = Expression.Lambda<Func<TSource, object>>(conversion, parameter);

            return Sort(source, sortingOptions.SortDirection, lambda);
        }
        catch (Exception ex)
        {
            throw new SortingException(ex.Message, ex);
        }
    }

    private static IOrderedQueryable<TSource> Sort<TBy>(IQueryable<TSource> collection, SortDirections sortDirection, Expression<Func<TSource, TBy>> expression)
    {
        return sortDirection == SortDirections.Ascending ? collection.OrderBy(expression) : collection.OrderByDescending(expression);
    }
}
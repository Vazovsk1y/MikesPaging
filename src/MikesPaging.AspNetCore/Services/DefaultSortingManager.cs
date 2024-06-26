﻿using Microsoft.Extensions.DependencyInjection;
using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Common.Interfaces;
using MikesPaging.AspNetCore.Exceptions;
using MikesPaging.AspNetCore.Exceptions.Base;
using MikesPaging.AspNetCore.Services.Interfaces;
using System.Linq.Expressions;

namespace MikesPaging.AspNetCore.Services;

public class DefaultSortingManager<TSource>(IServiceScopeFactory serviceScopeFactory) : ISortingManager<TSource>
{
    public IQueryable<TSource> ApplySorting<TSortBy>(IQueryable<TSource> source, SortingOptions<TSortBy>? sortingOptions)
        where TSortBy : SortingEnum
    {
        if (sortingOptions is null)
        {
            return source;
        }

        Validate(sortingOptions);

        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var configurationType = typeof(ISortingConfiguration<,>).MakeGenericType(typeof(TSource), typeof(TSortBy));
            var configuration = scope.ServiceProvider.GetService(configurationType);

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
        catch (Exception ex) when (ex is not MikesPagingException)
        {
            throw new SortingException(ex.Message, ex);
        }
    }

    private static IOrderedQueryable<TSource> Sort<TBy>(IQueryable<TSource> collection, SortingDirections sortDirection, Expression<Func<TSource, TBy>> expression)
    {
        return sortDirection == SortingDirections.Ascending ? collection.OrderBy(expression) : collection.OrderByDescending(expression);
    }

    private static void Validate<T>(SortingOptions<T> sortingOptions)
        where T : SortingEnum
    {
        SortingException.ThrowIf(sortingOptions.SortBy is null, Errors.ValueCannotBeNull(nameof(sortingOptions.SortBy)));
    }
}
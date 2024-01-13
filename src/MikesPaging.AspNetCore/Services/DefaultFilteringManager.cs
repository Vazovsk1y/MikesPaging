using Microsoft.Extensions.DependencyInjection;
using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Common.Interfaces;
using MikesPaging.AspNetCore.Exceptions;
using MikesPaging.AspNetCore.Services.Interfaces;
using System.Linq.Expressions;

namespace MikesPaging.AspNetCore.Services;

public class DefaultFilteringManager<TSource>(IServiceScopeFactory serviceScopeFactory) : IFilteringManager<TSource>
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    public IQueryable<TSource> ApplyFiltering<TFilterBy>(IQueryable<TSource> source, FilteringOptions<TFilterBy>? filteringOptions) 
        where TFilterBy : Enum
    {
        if (filteringOptions is null)
        {
            return source;
        }

        Validate(filteringOptions);

        Expression<Func<TSource, bool>> compositeFilterExpression = filteringOptions.Logic switch
        {
            Logic.And => GetAndFilterExpression(filteringOptions.Filters),
            Logic.Or => GetOrFilterExpression(filteringOptions.Filters),
            _ => throw new KeyNotFoundException(),
        };

        return source.Where(compositeFilterExpression);
    }

    private Expression<Func<TSource, bool>> GetAndFilterExpression<T>(IReadOnlyCollection<Filter<T>> filters)
        where T : Enum
    {
        var parameter = Expression.Parameter(typeof(TSource), "x");
        Expression? andExpression = null;

        foreach (var filter in filters)
        {
            var filterExpression = BuildFilterExpression(filter, parameter);
            if (andExpression is null)
            {
                andExpression = filterExpression;
            }
            else
            {
                andExpression = Expression.AndAlso(andExpression, filterExpression);
            }
        }

        if (andExpression is null)
        {
            throw new InvalidOperationException("Filters were not applied.");
        }

        return Expression.Lambda<Func<TSource, bool>>(andExpression, parameter);
    }

    private Expression<Func<TSource, bool>> GetOrFilterExpression<T>(IReadOnlyCollection<Filter<T>> filters)
        where T : Enum
    {
        var parameter = Expression.Parameter(typeof(TSource), "x");
        Expression? orExpression = null;

        foreach (var filter in filters)
        {
            var filterExpression = BuildFilterExpression(filter, parameter);
            if (orExpression is null)
            {
                orExpression = filterExpression;
            }
            else
            {
                orExpression = Expression.OrElse(orExpression, filterExpression);
            }
        }

        if (orExpression is null)
        {
            throw new InvalidOperationException("Filters were not applied.");
        }

        return Expression.Lambda<Func<TSource, bool>>(orExpression, parameter);
    }

    private Expression BuildFilterExpression<T>(Filter<T> filter, ParameterExpression parameter)
        where T : Enum
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var configurationType = typeof(IFilteringConfiguration<,>).MakeGenericType(typeof(TSource), typeof(T));
        var configuration = scope.ServiceProvider.GetService(configurationType);
        if (configuration is IFilteringConfiguration<TSource, T> castedConfiguration)
        {
            if (castedConfiguration.Filters.TryGetValue(new FilterKey<T>(filter.FilterBy, filter.Operator), out var configuredFilter))
            {
                try
                {
                    return configuredFilter.Invoke(filter.Value);
                }
                catch (Exception ex)
                {
                    throw new FilteringException(ex.Message);
                }
            }
        }

        var property = Expression.Property(parameter, filter.FilterBy.ToString());
        var convertionResult = Convert.ChangeType(filter.Value, property.Type)
            ?? throw new FilteringException($"Unable cast [{filter.Value}] to [{property.Type}].");

        var constant = Expression.Constant(convertionResult);
        switch (filter.Operator)
        {
            case FilteringOperators.NotEqual:
                return Expression.NotEqual(property, constant);
            case FilteringOperators.LessThanOrEqual:
                return Expression.LessThanOrEqual(property, constant);
            case FilteringOperators.GreaterThanOrEqual:
                return Expression.GreaterThanOrEqual(property, constant);
            case FilteringOperators.LessThan:
                return Expression.LessThan(property, constant);
            case FilteringOperators.GreaterThan:
                return Expression.GreaterThan(property, constant);
            case FilteringOperators.Contains:
                var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);
                return Expression.Call(property, containsMethod!, constant);
            case FilteringOperators.StartsWith:
                var startsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)]);
                return Expression.Call(property, startsWithMethod!, constant);
            default:
                throw new FilteringException($"Unsupported operator: [{filter.Operator}]."); ;
        }
    }

    private static void Validate<T>(FilteringOptions<T> filteringOptions)
        where T : Enum
    {
        FilteringException.ThrowIf(filteringOptions.Filters.Count == 0, "Filters collection cannot be empty.");

        HashSet<Filter<T>> filters = new(filteringOptions.Filters);
        FilteringException.ThrowIf(filters.Count != filteringOptions.Filters.Count, "Filters collection contain duplicates.");

        foreach (var item in filteringOptions.Filters)
        {
            FilteringException.ThrowIf(item.Value is null, "Filter value cannot be null.");
        }
    }
}
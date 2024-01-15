using Microsoft.Extensions.DependencyInjection;
using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Common.Interfaces;
using MikesPaging.AspNetCore.Exceptions;
using MikesPaging.AspNetCore.Services.Interfaces;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MikesPaging.AspNetCore.Services;

public sealed class DefaultFilteringManager<TSource>(IServiceScopeFactory serviceScopeFactory) : IFilteringManager<TSource>
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    public IQueryable<TSource> ApplyFiltering<TFilterBy>(IQueryable<TSource> source, FilteringOptions<TFilterBy>? filteringOptions) 
        where TFilterBy : FilteringEnum
    {
        if (filteringOptions is null)
        {
            return source;
        }

        Validate(filteringOptions);

        try
        {
            Expression<Func<TSource, bool>> compositeFilterExpression = filteringOptions.Logic switch
            {
                Logic.And => GetAndFilterExpression(filteringOptions.Filters),
                Logic.Or => GetOrFilterExpression(filteringOptions.Filters),
                _ => throw new KeyNotFoundException(),
            };

            return source.Where(compositeFilterExpression);
        }
        catch (Exception ex)
        {
            throw new FilteringException(ex.Message, ex);
        }
    }

    private Expression<Func<TSource, bool>> GetAndFilterExpression<T>(IReadOnlyCollection<Filter<T>> filters)
        where T : FilteringEnum
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

        return Expression.Lambda<Func<TSource, bool>>(andExpression!, parameter);
    }

    private Expression<Func<TSource, bool>> GetOrFilterExpression<T>(IReadOnlyCollection<Filter<T>> filters)
        where T : FilteringEnum
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

        return Expression.Lambda<Func<TSource, bool>>(orExpression!, parameter);
    }

    private Expression BuildFilterExpression<T>(Filter<T> filter, ParameterExpression parameter)
        where T : FilteringEnum
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var configurationType = typeof(IFilteringConfiguration<,>).MakeGenericType(typeof(TSource), typeof(T));
        var configuration = scope.ServiceProvider.GetService(configurationType);
        if (configuration is IFilteringConfiguration<TSource, T> castedConfiguration)
        {
            if (castedConfiguration.Filters.TryGetValue(new FilterKey<T>(filter.FilterBy, filter.Operator), out var configuredFilter))
            {
                return configuredFilter.Invoke(filter.Value) ?? 
                    throw new ArgumentNullException(Errors.ValueCannotBeNull("Filter expression"));
            }
        }

        var property = Expression.Property(parameter, filter.FilterBy.PropertyName);
        var convertionResult = TypeDescriptor.GetConverter(property.Type).ConvertFromInvariantString(filter.Value) 
            ?? throw new InvalidCastException($"Unable convert filter value {filter.Value} to {property.Type.Name} property type.");

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
                throw new KeyNotFoundException($"Unsupported operator: [{filter.Operator}]."); ;
        }
    }

    private static void Validate<T>(FilteringOptions<T> filteringOptions)
        where T : FilteringEnum
    {
        FilteringException.ThrowIf(filteringOptions.Filters is null || filteringOptions.Filters.Count == 0, Errors.ValueCannotBeNullOrEmpty("Filters collection"));
        FilteringException.ThrowIf(filteringOptions.Filters!.Distinct().Count() != filteringOptions.Filters!.Count, Errors.Filtering.FiltersCollectionCannotContainDuplicates);
        FilteringException.ThrowIf(filteringOptions.Filters.Any(e => e.Value is null), Errors.ValueCannotBeNull("Filter value"));
        FilteringException.ThrowIf(filteringOptions.Filters.Any(e => e is null), Errors.ValueCannotBeNull("Filter"));
    }
}
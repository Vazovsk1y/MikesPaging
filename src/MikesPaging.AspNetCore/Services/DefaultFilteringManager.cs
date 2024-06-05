using Microsoft.Extensions.DependencyInjection;
using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Common.Interfaces;
using MikesPaging.AspNetCore.Exceptions;
using MikesPaging.AspNetCore.Exceptions.Base;
using MikesPaging.AspNetCore.Services.Interfaces;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MikesPaging.AspNetCore.Services;

public sealed class DefaultFilteringManager<TSource>(IServiceScopeFactory serviceScopeFactory) : IFilteringManager<TSource>
{
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
        catch (Exception ex) when (ex is not MikesPagingException)
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
            andExpression = andExpression is null ? filterExpression : Expression.AndAlso(andExpression, filterExpression);
        }

        ArgumentNullException.ThrowIfNull(andExpression);
        return Expression.Lambda<Func<TSource, bool>>(andExpression, parameter);
    }

    private Expression<Func<TSource, bool>> GetOrFilterExpression<T>(IReadOnlyCollection<Filter<T>> filters)
        where T : FilteringEnum
    {
        var parameter = Expression.Parameter(typeof(TSource), "x");
        Expression? orExpression = null;

        foreach (var filter in filters)
        {
            var filterExpression = BuildFilterExpression(filter, parameter);
            orExpression = orExpression is null ? filterExpression : Expression.OrElse(orExpression, filterExpression);
        }

        ArgumentNullException.ThrowIfNull(orExpression);
        return Expression.Lambda<Func<TSource, bool>>(orExpression, parameter);
    }

    private Expression BuildFilterExpression<T>(Filter<T> filter, ParameterExpression parameter)
        where T : FilteringEnum
    {
        FilteringException.ThrowIf(!filter.FilterBy.IsOperatorApplicable(filter.Operator), Errors.Filtering.OperatorIsNotApplicableFor(filter.FilterBy, filter.Operator));

        using var scope = serviceScopeFactory.CreateScope();
        var configurationType = typeof(IFilteringConfiguration<,>).MakeGenericType(typeof(TSource), typeof(T));
        var configuration = scope.ServiceProvider.GetService(configurationType);
        if (configuration is IFilteringConfiguration<TSource, T> castedConfiguration)
        {
            if (castedConfiguration.Filters.TryGetValue(new FilterKey<T>(filter.FilterBy, filter.Operator), out var configuredFilter))
            {
                var filterExpression = configuredFilter.Invoke(filter.Value);
                ArgumentNullException.ThrowIfNull(filterExpression);
                return Expression.Invoke(filterExpression, parameter);
            }
        }

        var property = Expression.Property(parameter, filter.FilterBy.PropertyName);
        var convertedProperty = Expression.Convert(property, property.Type);

        var convertedFilterValue = filter.Value is null ?
            null
            :
            TypeDescriptor.GetConverter(convertedProperty.Type).ConvertFromInvariantString(filter.Value)
            ?? throw new InvalidCastException($"Unable convert filter value '{filter.Value}' to '{convertedProperty.Type.Name}' property type.");

        var constant = Expression.Constant(convertedFilterValue);
        var convertedConstant = Expression.Convert(constant, convertedProperty.Type);

        switch (filter.Operator)
        {
            case FilteringOperators.NotEqual:
                return Expression.NotEqual(convertedProperty, convertedConstant);
            case FilteringOperators.LessThanOrEqual:
                return Expression.LessThanOrEqual(convertedProperty, convertedConstant);
            case FilteringOperators.GreaterThanOrEqual:
                return Expression.GreaterThanOrEqual(convertedProperty, convertedConstant);
            case FilteringOperators.LessThan:
                return Expression.LessThan(convertedProperty, convertedConstant);
            case FilteringOperators.GreaterThan:
                return Expression.GreaterThan(convertedProperty, convertedConstant);
            case FilteringOperators.Contains:
                var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);
                return Expression.Call(convertedProperty, containsMethod!, convertedConstant);
            case FilteringOperators.StartsWith:
                var startsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)]);
                return Expression.Call(convertedProperty, startsWithMethod!, convertedConstant);
            default:
                throw new KeyNotFoundException($"Unsupported operator: [{filter.Operator}]."); ;
        }
    }

    private static void Validate<T>(FilteringOptions<T> filteringOptions)
        where T : FilteringEnum
    {
        FilteringException.ThrowIf(filteringOptions.Filters is null || filteringOptions.Filters.Count == 0, Errors.ValueCannotBeNullOrEmpty(nameof(filteringOptions.Filters)));
        FilteringException.ThrowIf(filteringOptions.Filters.Distinct().Count() != filteringOptions.Filters.Count, Errors.Filtering.FiltersCollectionCannotContainDuplicates);
        FilteringException.ThrowIf(filteringOptions.Filters.Any(e => e is null), Errors.ValueCannotBeNull(nameof(Filter<T>)));
        FilteringException.ThrowIf(filteringOptions.Filters.Any(e => e.FilterBy is null), Errors.ValueCannotBeNull(nameof(Filter<T>.FilterBy)));
    }
}
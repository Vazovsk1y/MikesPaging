using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Common.Interfaces;
using MikesPaging.AspNetCore.Exceptions;
using System.Linq.Expressions;

namespace MikesPaging.AspNetCore.Common;

/// <summary>
/// Base class for configuring filtering options for a specific data source.
/// </summary>
/// <typeparam name="TSource">The type of the data source.</typeparam>
/// <typeparam name="TFilterBy">The type of the filtering criteria.</typeparam>
public abstract class FilteringConfiguration<TSource, TFilterBy> : IFilteringConfiguration<TSource, TFilterBy>
    where TFilterBy : FilteringEnum
{
    private readonly Dictionary<FilterKey<TFilterBy>, Func<string?, Expression<Func<TSource, bool>>>> _filters = new();

    /// <summary>
    /// Gets the dictionary of filtering keys and their corresponding expressions.
    /// </summary>
    public IReadOnlyDictionary<FilterKey<TFilterBy>, Func<string?, Expression<Func<TSource, bool>>>> Filters => _filters.AsReadOnly();

    /// <summary>
    /// Configures a filter for the specified filtering criterion and operator.
    /// </summary>
    /// <param name="filterBy">The filtering criterion.</param>
    /// <param name="operator">The operator to apply.</param>
    /// <param name="filter">The function providing the filtering expression, <c>string?</c> function argument is filter value.</param>
    protected void FilterFor(TFilterBy filterBy, FilteringOperators @operator, Func<string?, Expression<Func<TSource, bool>>> filter)
    {
        FilteringException.ThrowIf(filterBy is null, Errors.ValueCannotBeNull(nameof(filterBy)));
        FilteringException.ThrowIf(filter is null, Errors.ValueCannotBeNull(nameof(filter)));
        FilteringException.ThrowIf(!filterBy.IsOperatorApplicable(@operator), Errors.Filtering.OperatorIsNotApplicableFor(filterBy, @operator));

        _filters[new FilterKey<TFilterBy>(filterBy, @operator)] = filter;
    }
}

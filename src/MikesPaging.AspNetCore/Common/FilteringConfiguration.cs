using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Common.Interfaces;
using MikesPaging.AspNetCore.Exceptions;
using System.Linq.Expressions;

namespace MikesPaging.AspNetCore.Common;

public abstract class FilteringConfiguration<TSource, TFilterBy> : IFilteringConfiguration<TSource, TFilterBy> 
    where TFilterBy : Enum
{
    private readonly Dictionary<FilterKey<TFilterBy>, Func<string, Expression<Func<TSource, bool>>>> _filters = [];
    public IReadOnlyDictionary<FilterKey<TFilterBy>, Func<string, Expression<Func<TSource, bool>>>> Filters => _filters.AsReadOnly();

    protected void RuleFor(TFilterBy filterBy, FilteringOperators @operator, Func<string, Expression<Func<TSource, bool>>> value)
    {
        FilteringException.ThrowIf(filterBy is null, Errors.ValueCannotBeNull("Filter by"));
        FilteringException.ThrowIf(value is null, Errors.ValueCannotBeNull("Filtering delegate"));
        _filters[new FilterKey<TFilterBy>(filterBy!, @operator)] = value!;
    }
}
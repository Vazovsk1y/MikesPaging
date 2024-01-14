using MikesPaging.AspNetCore.Common.Interfaces;
using MikesPaging.AspNetCore.Exceptions;
using System.Linq.Expressions;

namespace MikesPaging.AspNetCore.Common;

public abstract class SortingConfiguration<TSource, TSortBy> : ISortingConfiguration<TSource, TSortBy> 
    where TSortBy : MikesPagingEnum
{
    private readonly Dictionary<TSortBy, Expression<Func<TSource, object>>> _sorters = [];
    public IReadOnlyDictionary<TSortBy, Expression<Func<TSource, object>>> Sorters => _sorters.AsReadOnly();

    protected void RuleFor(TSortBy key, Expression<Func<TSource, object>> value)
    {
        SortingException.ThrowIf(key is null, Errors.ValueCannotBeNull("Sort by key value"));
        SortingException.ThrowIf(value is null, Errors.ValueCannotBeNull("Sorting expression"));

        _sorters[key!] = value!;
    }
}
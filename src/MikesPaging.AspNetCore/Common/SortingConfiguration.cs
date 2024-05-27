using MikesPaging.AspNetCore.Common.Interfaces;
using MikesPaging.AspNetCore.Exceptions;
using System.Linq.Expressions;

namespace MikesPaging.AspNetCore.Common;

/// <summary>
/// Base class for configuring sorting options for a specific data source.
/// </summary>
/// <typeparam name="TSource">The type of the data source.</typeparam>
/// <typeparam name="TSortBy">The type of the sorting criteria.</typeparam>
public abstract class SortingConfiguration<TSource, TSortBy> : ISortingConfiguration<TSource, TSortBy>
    where TSortBy : SortingEnum
{
    private readonly Dictionary<TSortBy, Expression<Func<TSource, object>>> _sorters = new();

    /// <summary>
    /// Gets the dictionary of sorting keys and their corresponding expressions.
    /// </summary>
    public IReadOnlyDictionary<TSortBy, Expression<Func<TSource, object>>> Sorters => _sorters.AsReadOnly();

    /// <summary>
    /// Configures a sorter for the specified sorting key.
    /// </summary>
    /// <param name="key">The sorting key.</param>
    /// <param name="sorter">The expression providing the sorting logic.</param>
    protected void SortFor(TSortBy key, Expression<Func<TSource, object>> sorter)
    {
        SortingException.ThrowIf(key is null, Errors.ValueCannotBeNull(nameof(key)));
        SortingException.ThrowIf(sorter is null, Errors.ValueCannotBeNull(nameof(sorter)));

        _sorters[key] = sorter;
    }
}
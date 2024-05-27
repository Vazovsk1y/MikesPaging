using MikesPaging.AspNetCore.Common.Enums;
using System.Linq.Expressions;

namespace MikesPaging.AspNetCore.Common.Interfaces;

/// <summary>
/// Represents a configuration for filtering a collection of <typeparamref name="TSource"/> based on criteria defined by <typeparamref name="TFilterBy"/>.
/// </summary>
/// <typeparam name="TSource">The type of the elements in the collection to be filtered.</typeparam>
/// <typeparam name="TFilterBy">The type of the filtering criteria, which must derive from <see cref="FilteringEnum"/>.</typeparam>
public interface IFilteringConfiguration<TSource, TFilterBy>
    where TFilterBy : FilteringEnum
{
    /// <summary>
    /// Gets a dictionary of filter keys and their corresponding filtering expressions.
    /// </summary>
    /// <value>
    /// A read-only dictionary where the key is a <see cref="FilterKey{TFilterBy}"/> and the value is a function that takes an optional string parameter 
    /// and returns an expression used to filter the <typeparamref name="TSource"/> collection.
    /// </value>
    IReadOnlyDictionary<FilterKey<TFilterBy>, Func<string?, Expression<Func<TSource, bool>>>> Filters { get; }
}

/// <summary>
/// Represents a key used for filtering, consisting of a filtering criterion and an operator.
/// </summary>
/// <typeparam name="T">The type of the filtering criterion, which must derive from <see cref="FilteringEnum"/>.</typeparam>
public record FilterKey<T>(T FilterBy, FilteringOperators Operator)
    where T : FilteringEnum;

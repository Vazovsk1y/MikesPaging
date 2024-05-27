using System.Linq.Expressions;

namespace MikesPaging.AspNetCore.Common.Interfaces;

/// <summary>
/// Represents a configuration for sorting a collection of <typeparamref name="TSource"/> based on criteria defined by <typeparamref name="TSortBy"/>.
/// </summary>
/// <typeparam name="TSource">The type of the elements in the collection to be sorted.</typeparam>
/// <typeparam name="TSortBy">The type of the sorting criteria, which must derive from <see cref="SortingEnum"/>.</typeparam>
public interface ISortingConfiguration<TSource, TSortBy>
    where TSortBy : SortingEnum
{
    /// <summary>
    /// Gets a dictionary of sorting keys and their corresponding sorting expressions.
    /// </summary>
    /// <value>
    /// A read-only dictionary where the key is of type <typeparamref name="TSortBy"/> and the value is an expression used to sort the <typeparamref name="TSource"/> collection.
    /// </value>
    IReadOnlyDictionary<TSortBy, Expression<Func<TSource, object>>> Sorters { get; }
}

using MikesPaging.AspNetCore.Common;

namespace MikesPaging.AspNetCore.Services.Interfaces;

/// <summary>
/// Interface for managing sorting operations on a data source.
/// </summary>
/// <typeparam name="TSource">The type of the data source.</typeparam>
public interface ISortingManager<TSource>
{
    /// <summary>
    /// Applies sorting to the specified data source based on the provided sorting options.
    /// </summary>
    /// <typeparam name="TSortBy">The type of the sorting criteria.</typeparam>
    /// <param name="source">The data source to apply sorting to.</param>
    /// <param name="sortingOptions">The sorting options to apply. If <c>null</c>, no sorting is applied.</param>
    /// <returns>A sorted <see cref="IQueryable{TSource}"/>.</returns>
    IQueryable<TSource> ApplySorting<TSortBy>(IQueryable<TSource> source, SortingOptions<TSortBy>? sortingOptions)
        where TSortBy : SortingEnum;
}
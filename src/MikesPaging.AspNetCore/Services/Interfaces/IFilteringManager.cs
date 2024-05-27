using MikesPaging.AspNetCore.Common;

namespace MikesPaging.AspNetCore.Services.Interfaces;

/// <summary>
/// Interface for managing filtering operations on a data source.
/// </summary>
/// <typeparam name="TSource">The type of the data source.</typeparam>
public interface IFilteringManager<TSource>
{
    /// <summary>
    /// Applies filtering to the specified data source based on the provided filtering options.
    /// </summary>
    /// <typeparam name="TFilterBy">The type of the filtering criteria.</typeparam>
    /// <param name="source">The data source to apply filtering to.</param>
    /// <param name="filteringOptions">The filtering options to apply.</param>
    /// <returns>A filtered <see cref="IQueryable{TSource}"/>.</returns>
    IQueryable<TSource> ApplyFiltering<TFilterBy>(IQueryable<TSource> source, FilteringOptions<TFilterBy>? filteringOptions)
        where TFilterBy : FilteringEnum;
}
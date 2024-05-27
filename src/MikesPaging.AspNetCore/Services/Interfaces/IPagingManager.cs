using MikesPaging.AspNetCore.Common;

namespace MikesPaging.AspNetCore.Services.Interfaces;

/// <summary>
/// Interface for managing paging operations on a data source.
/// </summary>
/// <typeparam name="TSource">The type of the data source.</typeparam>
public interface IPagingManager<TSource>
{
    /// <summary>
    /// Applies paging to the specified data source based on the provided paging options.
    /// </summary>
    /// <param name="source">The data source to apply paging to.</param>
    /// <param name="pagingOptions">The paging options to apply. If <c>null</c>, no paging is applied.</param>
    /// <returns>A paged <see cref="IQueryable{TSource}"/>.</returns>
    IQueryable<TSource> ApplyPaging(IQueryable<TSource> source, PagingOptions? pagingOptions);
}
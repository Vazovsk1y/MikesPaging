using MikesPaging.AspNetCore.Common;

namespace MikesPaging.AspNetCore.Services.Interfaces;

public interface IFilteringManager<TSource>
{
    IQueryable<TSource> ApplyFiltering<TFilterBy>(IQueryable<TSource> source, FilteringOptions<TFilterBy>? filteringOptions) 
        where TFilterBy : Enum;
}
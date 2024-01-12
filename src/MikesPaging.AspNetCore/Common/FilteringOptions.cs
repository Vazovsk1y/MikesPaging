using MikesPaging.AspNetCore.Common.Enums;

namespace MikesPaging.AspNetCore.Common;

public interface IFilteringOptions { }
public record FilteringOptions<TFilterBy>(IReadOnlyCollection<Filter<TFilterBy>> Filters, Logic Logic) : IFilteringOptions
    where TFilterBy : Enum;
using MikesPaging.Core.Common.Enums;

namespace MikesPaging.Core.Common;

public interface IFilteringOptions { }
public record FilteringOptions<TFilterBy>(IReadOnlyCollection<Filter<TFilterBy>> Filters, Logic Logic) : IFilteringOptions
    where TFilterBy : Enum;
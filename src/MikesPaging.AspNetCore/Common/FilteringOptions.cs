using MikesPaging.AspNetCore.Common.Enums;
using System.Text.Json.Serialization;

namespace MikesPaging.AspNetCore.Common;

public interface IFilteringOptions { }
public record FilteringOptions<TFilterBy>(
    IReadOnlyCollection<Filter<TFilterBy>> Filters,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    Logic Logic) : IFilteringOptions
    where TFilterBy : Enum;
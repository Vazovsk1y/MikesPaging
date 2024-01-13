using MikesPaging.AspNetCore.Common.Enums;
using System.Text.Json.Serialization;

namespace MikesPaging.AspNetCore.Common;

public record Filter<TFilterBy>(
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    TFilterBy FilterBy,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    FilteringOperators Operator, 
    string Value);

using MikesPaging.AspNetCore.Common.Enums;
using System.Text.Json.Serialization;

namespace MikesPaging.AspNetCore.Common;

public interface ISortingOptions { }
public record SortingOptions<TSortBy>(
    [property: JsonConverter(typeof(JsonStringEnumConverter))] 
    SortDirections SortDirection, 
    [property: JsonConverter(typeof(JsonStringEnumConverter))] 
    TSortBy SortBy) : ISortingOptions
    where TSortBy : Enum;
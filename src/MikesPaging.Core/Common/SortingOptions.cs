using MikesPaging.Core.Common.Enums;

namespace MikesPaging.Core.Common;

public interface ISortingOptions { }
public record SortingOptions<TSortBy>(SortDirections SortDirection, TSortBy SortBy) : ISortingOptions
    where TSortBy : Enum;
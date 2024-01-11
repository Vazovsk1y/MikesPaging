using MikesPaging.Core.Common.Enums;

namespace MikesPaging.Core.Common.Interfaces;

public interface ISortingOptions
{

}

public interface ISortingOptions<T> : ISortingOptions where T : Enum
{
    SortDirections SortDirection { get; }

    T SortBy { get; }
}
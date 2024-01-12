using MikesPaging.Core.Common.Enums;

namespace MikesPaging.Core.Common.Interfaces;

public interface ISortingOptions
{

}

public interface ISortingOptions<TSortBy> : ISortingOptions 
    where TSortBy : Enum
{
    SortDirections SortDirection { get; }

    TSortBy SortBy { get; }
}
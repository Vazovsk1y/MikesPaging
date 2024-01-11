namespace MikesPaging.Core.Common.Interfaces;

public interface ISortedPage<TItem, TSortingOptions> : IPage<TItem>
    where TSortingOptions : class, ISortingOptions
{
    TSortingOptions? AppliedSorting { get; }
}
using System.Linq.Expressions;

namespace MikesPaging.AspNetCore.Common.Interfaces;

public interface ISortingConfiguration<TSource, TSortBy> 
    where TSortBy : Enum
{
    IReadOnlyDictionary<TSortBy, Expression<Func<TSource, object>>> Sorters { get; }
}
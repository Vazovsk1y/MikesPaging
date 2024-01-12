using System.Linq.Expressions;

namespace MikesPaging.Core.Common.Interfaces;

public interface ISortingConfiguration<TSource, TSortBy> 
    where TSortBy : Enum
{
    IReadOnlyDictionary<TSortBy, Expression<Func<TSource, object>>> Sorters { get; }
}
using System.Linq.Expressions;

namespace MikesPaging.Core.Common.Interfaces;

public interface ISortingConfiguration<TSource, T> where T : Enum
{
    IReadOnlyDictionary<T, Expression<Func<TSource, object>>> Sorters { get; }
}
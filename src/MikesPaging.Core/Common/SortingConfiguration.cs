using MikesPaging.Core.Common.Interfaces;
using MikesPaging.Core.Exceptions;
using System.Linq.Expressions;

namespace MikesPaging.Core.Common;

public abstract class SortingConfiguration<TSource, T> : ISortingConfiguration<TSource, T> where T : Enum
{
    private readonly Dictionary<T, Expression<Func<TSource, object>>> _sorters = [];
    public IReadOnlyDictionary<T, Expression<Func<TSource, object>>> Sorters => _sorters.AsReadOnly();

    protected void RuleFor(T key, Expression<Func<TSource, object>> value)
    {
        SortingException.ThrowIf(key is null, "Key is required and can't be null.");
        SortingException.ThrowIf(value is null, "Expression can't be null.");

        _sorters[key!] = value!;
    }
}
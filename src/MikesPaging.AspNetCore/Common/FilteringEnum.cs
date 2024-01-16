using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Exceptions;

namespace MikesPaging.AspNetCore.Common;

public abstract class FilteringEnum : MikesPagingEnum
{
    public IReadOnlyCollection<FilteringOperators> InapplicableOperators { get; }

    protected FilteringEnum(string propertyName, IReadOnlyCollection<string> allowedNames, bool ignoreCase = true, IReadOnlyCollection<FilteringOperators>? inapplicableOperators = null)
        : base(propertyName, allowedNames, ignoreCase)
    {
        if (inapplicableOperators is null)
        {
            InapplicableOperators = [];
        }
        else
        {
            FilteringException.ThrowIf(inapplicableOperators.Distinct().Count() != inapplicableOperators.Count, "Inapplicable operators collection contains duplicates.");
            InapplicableOperators = inapplicableOperators;
        }
    }

    protected sealed override IEnumerable<object?> GetEqualityComponents()
    {
        foreach (var item in base.GetEqualityComponents())
        {
            yield return item;
        }

        foreach (var @operator in InapplicableOperators.OrderBy(e => e))
        {
            yield return @operator;
        }
    }
}
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Exceptions;
using System.Reflection;

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

    public static IEnumerable<T> Enumerate<T>()
        where T : FilteringEnum
    {
        var enumType = typeof(T);
        var fields = enumType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic)
            .Where(f => f.FieldType == enumType && (f.IsPublic || f.IsAssembly) && f.IsInitOnly)
            .OrderBy(e => e.Name);

        foreach (var field in fields)
        {
            var fieldValue = field.GetValue(null) as T;
            if (fieldValue is not null)
            {
                yield return fieldValue;
            }
        }
    }

    public static T? FindFirstOrDefault<T>(string name)
        where T : FilteringEnum
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return Enumerate<T>()
              .FirstOrDefault(e =>
                     e.AllowedNames.Contains(name, e.IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal));
    }

    public bool IsOperatorApplicable(FilteringOperators @operator)
    {
        return !InapplicableOperators.Contains(@operator);
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
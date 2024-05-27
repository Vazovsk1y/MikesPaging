using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Exceptions;
using System.Reflection;
using System.Text.Json.Serialization;

namespace MikesPaging.AspNetCore.Common;

/// <summary>
/// Base class representing an enumeration of filtering criteria.
/// </summary>
public abstract class FilteringEnum : MikesPagingEnum
{
    /// <summary>
    /// Gets the collection of operators that are not applicable to this filtering criterion.
    /// </summary>
    /// 
    [JsonConverter(typeof(JsonReadOnlyCollectionItemConverter<FilteringOperators, JsonStringEnumConverter>))]
    public IReadOnlyCollection<FilteringOperators> InapplicableOperators { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilteringEnum"/> class with the specified parameters.
    /// </summary>
    /// <param name="propertyName">The name of the property associated with this filtering criterion.</param>
    /// <param name="allowedNames">The allowed names for this filtering criterion.</param>
    /// <param name="ignoreCase">Specifies whether to ignore case when comparing names.</param>
    /// <param name="inapplicableOperators">The collection of operators that are not applicable to this filtering criterion.</param>
    protected FilteringEnum(
        string propertyName,
        IReadOnlyCollection<string> allowedNames,
        bool ignoreCase = true,
        IReadOnlyCollection<FilteringOperators>? inapplicableOperators = null)
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

    /// <summary>
    /// Enumerates all values of the specified <typeparamref name="T"/> enumeration.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <returns>An enumerable collection of enumeration values.</returns>
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
            if (field.GetValue(null) is T fieldValue)
            {
                yield return fieldValue;
            }
        }
    }

    /// <summary>
    /// Finds the first filtering criterion of type <typeparamref name="T"/> with the specified name.
    /// </summary>
    /// <typeparam name="T">The type of the filtering criterion.</typeparam>
    /// <param name="name">The name of the filtering criterion to find.</param>
    /// <returns>The first filtering criterion with the specified name, or <c>null</c> if not found.</returns>
    public static T? FindFirstOrDefault<T>(string name)
        where T : FilteringEnum
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return Enumerate<T>()
              .FirstOrDefault(e =>
                     e.AllowedNames.Contains(name, e.IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal));
    }

    /// <summary>
    /// Checks if the specified operator is applicable to this filtering criterion.
    /// </summary>
    /// <param name="operator">The operator to check.</param>
    /// <returns><c>true</c> if the operator is applicable; otherwise, <c>false</c>.</returns>
    public bool IsOperatorApplicable(FilteringOperators @operator)
    {
        return !InapplicableOperators.Contains(@operator);
    }

    /// <summary>
    /// Gets the equality components for the <see cref="FilteringEnum"/>.
    /// </summary>
    /// <returns>An enumerable collection of equality components.</returns>
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

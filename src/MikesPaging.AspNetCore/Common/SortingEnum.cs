using System.Reflection;

namespace MikesPaging.AspNetCore.Common;

/// <summary>
/// Base class representing an enumeration of sorting criteria.
/// </summary>
public abstract class SortingEnum : MikesPagingEnum
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortingEnum"/> class with the specified parameters.
    /// </summary>
    /// <param name="propertyName">The name of the property associated with this sorting criterion.</param>
    /// <param name="allowedNames">The allowed names for this sorting criterion.</param>
    protected SortingEnum(
        string propertyName,
        IReadOnlyCollection<string> allowedNames) : base(propertyName, allowedNames)
    {
    }

    /// <summary>
    /// Enumerates all values of the specified <typeparamref name="T"/> sorting enumeration.
    /// </summary>
    /// <typeparam name="T">The type of the sorting enumeration.</typeparam>
    /// <returns>An enumerable collection of sorting enumeration values.</returns>
    public static IEnumerable<T> Enumerate<T>()
        where T : SortingEnum
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
    /// Finds the first sorting criterion of type <typeparamref name="T"/> with the specified name.
    /// </summary>
    /// <typeparam name="T">The type of the sorting criterion.</typeparam>
    /// <param name="name">The name of the sorting criterion to find.</param>
    /// <returns>The first sorting criterion with the specified name, or <c>null</c> if not found.</returns>
    public static T? FindFirstOrDefault<T>(string name)
        where T : SortingEnum
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return Enumerate<T>()
            .FirstOrDefault(e =>
                e.AllowedNames.Contains(name, StringComparer.Ordinal));
    }

    /// <summary>
    /// Gets the equality components for the <see cref="SortingEnum"/>.
    /// </summary>
    /// <returns>An enumerable collection of equality components.</returns>
    protected sealed override IEnumerable<object?> GetEqualityComponents()
    {
        return base.GetEqualityComponents();
    }
}
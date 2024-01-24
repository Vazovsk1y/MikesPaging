using System.Reflection;

namespace MikesPaging.AspNetCore.Common;

public abstract class SortingEnum : MikesPagingEnum
{
    protected SortingEnum(string propertyName, IReadOnlyCollection<string> allowedNames) : base(propertyName, allowedNames)
    {

    }

    protected SortingEnum(string propertyName, IReadOnlyCollection<string> allowedNames, bool ignoreCase = true) : base(propertyName, allowedNames, ignoreCase)
    {

    }

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
            var fieldValue = field.GetValue(null) as T;
            if (fieldValue is not null)
            {
                yield return fieldValue;
            }
        }
    }

    public static T? FindFirstOrDefault<T>(string name)
        where T : SortingEnum
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return Enumerate<T>()
              .FirstOrDefault(e =>
                     e.AllowedNames.Contains(name, e.IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal));
    }
    protected sealed override IEnumerable<object?> GetEqualityComponents()
    {
        return base.GetEqualityComponents();
    }
}
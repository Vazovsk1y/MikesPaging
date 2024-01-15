using MikesPaging.AspNetCore.Exceptions.Base;
using System.Reflection;

namespace MikesPaging.AspNetCore.Common;

public abstract class MikesPagingEnum
{
    public string PropertyName { get; }

    public IReadOnlyCollection<string> AllowedNames { get; }

    private readonly bool _ignoreCase = true;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private MikesPagingEnum() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected MikesPagingEnum(string propertyName, IReadOnlyCollection<string> allowedNames, bool ignoreCase = true) 
    {
        MikesPagingException.ThrowIf(string.IsNullOrWhiteSpace(propertyName), Errors.ValueCannotBeNullOrEmpty("Property name"));
        MikesPagingException.ThrowIf(allowedNames is null || allowedNames.Count == 0, Errors.ValueCannotBeNullOrEmpty("Allowed names"));
        MikesPagingException.ThrowIf(allowedNames!.Any(string.IsNullOrWhiteSpace), "Allowed names collection cannot contain null or empty string.");
        MikesPagingException.ThrowIf(allowedNames!.Distinct().Count() != allowedNames!.Count, "Allowed names collection contain duplicates.");

        _ignoreCase = ignoreCase;
        PropertyName = propertyName;
        AllowedNames = allowedNames!;
    }

    public static IEnumerable<T> Enumerate<T>()
        where T : MikesPagingEnum
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
        where T : MikesPagingEnum
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return Enumerate<T>()
              .FirstOrDefault(e => 
                     e.AllowedNames.Contains(name, e._ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal));
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (GetType() != obj.GetType())
            return false;

        if (obj is not MikesPagingEnum @enum)
            return false;

        return GetEqualityComponents().SequenceEqual(@enum.GetEqualityComponents());
    }

    public override int GetHashCode() =>
        GetEqualityComponents().Aggregate(1, (current, obj) =>
        {
            unchecked
            {
                return current * 44 + (obj?.GetHashCode() ?? 0);
            }
        });

    private IEnumerable<object?> GetEqualityComponents()
    {
        yield return PropertyName;
        foreach (var item in AllowedNames)
        {
            yield return item;
        }
    }
}
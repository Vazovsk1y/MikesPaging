using MikesPaging.AspNetCore.Exceptions.Base;

namespace MikesPaging.AspNetCore.Common;

public abstract class MikesPagingEnum
{
    public string PropertyName { get; }

    public IReadOnlyCollection<string> AllowedNames { get; }

    public bool IgnoreCase { get; } = true;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private MikesPagingEnum() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected MikesPagingEnum(string propertyName, IReadOnlyCollection<string> allowedNames, bool ignoreCase = true) 
    {
        MikesPagingException.ThrowIf(string.IsNullOrWhiteSpace(propertyName), Errors.ValueCannotBeNullOrEmpty("Property name"));
        MikesPagingException.ThrowIf(allowedNames is null || allowedNames.Count == 0, Errors.ValueCannotBeNullOrEmpty("Allowed names"));
        MikesPagingException.ThrowIf(allowedNames!.Any(string.IsNullOrWhiteSpace), "Allowed names collection cannot contain null or empty string.");
        MikesPagingException.ThrowIf(allowedNames!.Distinct().Count() != allowedNames!.Count, "Allowed names collection contain duplicates.");

        IgnoreCase = ignoreCase;
        PropertyName = propertyName;
        AllowedNames = allowedNames!;
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

    protected virtual IEnumerable<object?> GetEqualityComponents()
    {
        yield return PropertyName;
        yield return IgnoreCase;
        foreach (var item in AllowedNames.OrderBy(e => e))
        {
            yield return item;
        }
    }
}
using MikesPaging.AspNetCore.Exceptions.Base;

namespace MikesPaging.AspNetCore.Common;

/// <summary>
/// Base class representing an enumeration of filtering or sorting criteria.
/// </summary>
public abstract class MikesPagingEnum
{
    /// <summary>
    /// Gets the name of the property associated with this enumeration.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the allowed names for this enumeration.
    /// </summary>
    public IReadOnlyCollection<string> AllowedNames { get; }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore case when comparing names.
    /// </summary>
    public bool IgnoreCase { get; } = true;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private MikesPagingEnum() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Initializes a new instance of the <see cref="MikesPagingEnum"/> class with the specified parameters.
    /// </summary>
    /// <param name="propertyName">The name of the property associated with this enumeration.</param>
    /// <param name="allowedNames">The allowed names for this enumeration.</param>
    /// <param name="ignoreCase">Specifies whether to ignore case when comparing names.</param>
    protected MikesPagingEnum(string propertyName, IReadOnlyCollection<string> allowedNames, bool ignoreCase = true)
    {
        MikesPagingException.ThrowIf(string.IsNullOrWhiteSpace(propertyName), Errors.ValueCannotBeNullOrEmpty(nameof(propertyName)));
        MikesPagingException.ThrowIf(allowedNames is null || allowedNames.Count == 0, Errors.ValueCannotBeNullOrEmpty(nameof(allowedNames)));
        MikesPagingException.ThrowIf(allowedNames.Any(string.IsNullOrWhiteSpace), "Allowed names collection cannot contain null or empty string.");
        MikesPagingException.ThrowIf(allowedNames.Distinct().Count() != allowedNames.Count, "Allowed names collection contain duplicates.");

        IgnoreCase = ignoreCase;
        PropertyName = propertyName;
        AllowedNames = allowedNames;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() =>
        GetEqualityComponents().Aggregate(1, (current, obj) =>
        {
            unchecked
            {
                return current * 44 + (obj?.GetHashCode() ?? 0);
            }
        });

    /// <summary>
    /// Gets the equality components for the <see cref="MikesPagingEnum"/>.
    /// </summary>
    /// <returns>An enumerable collection of equality components.</returns>
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

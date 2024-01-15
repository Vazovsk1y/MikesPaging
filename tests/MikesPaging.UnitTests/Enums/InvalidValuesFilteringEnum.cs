using MikesPaging.AspNetCore.Common;

namespace MikesPaging.UnitTests.Enums;

internal class InvalidValuesFilteringEnum : FilteringEnum
{
    public static InvalidValuesFilteringEnum DuplicatesInAllowedNames = new(nameof(DuplicatesInAllowedNames), [nameof(DuplicatesInAllowedNames), nameof(DuplicatesInAllowedNames)]);

    public static InvalidValuesFilteringEnum NullPassedToPropertyName = new(null, [nameof(NullPassedToPropertyName)]);

    public static InvalidValuesFilteringEnum EmptyStringPassedToPropertyName = new(string.Empty, [nameof(EmptyStringPassedToPropertyName)]);

    public static InvalidValuesFilteringEnum NullPassedToAllowedNames = new(nameof(NullPassedToAllowedNames), null);

    public static InvalidValuesFilteringEnum EmptyCollectionPassedToAllowedNames = new(nameof(EmptyCollectionPassedToAllowedNames), []);

    public static InvalidValuesFilteringEnum NullContainsInAllowedNames = new(nameof(NullContainsInAllowedNames), [nameof(NullPassedToPropertyName), null]);

    public static InvalidValuesFilteringEnum EmptyStringContainsInAllowedNames = new(nameof(EmptyStringContainsInAllowedNames), [nameof(EmptyStringContainsInAllowedNames), string.Empty]);
    private InvalidValuesFilteringEnum(string propertyName, IReadOnlyCollection<string> allowedNames) : base(propertyName, allowedNames)
    {
    }
}

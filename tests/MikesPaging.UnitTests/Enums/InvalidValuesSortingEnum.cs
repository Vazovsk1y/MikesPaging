using MikesPaging.AspNetCore.Common;
using MikesPaging.UnitTests.Mapper;

namespace MikesPaging.UnitTests.Enums;

internal class InvalidValuesSortingEnum : SortingEnum
{
    public static InvalidValuesSortingEnum DuplicatesInAllowedNames = new (nameof(DuplicatesInAllowedNames), [nameof(DuplicatesInAllowedNames), nameof(DuplicatesInAllowedNames)]);

    public static InvalidValuesSortingEnum NullPassedToPropertyName = new(null, [nameof(NullPassedToPropertyName)]);

    public static InvalidValuesSortingEnum EmptyStringPassedToPropertyName = new(string.Empty, [nameof(EmptyStringPassedToPropertyName)]);

    public static InvalidValuesSortingEnum NullPassedToAllowedNames = new(nameof(NullPassedToAllowedNames), null);

    public static InvalidValuesSortingEnum EmptyCollectionPassedToAllowedNames = new(nameof(EmptyCollectionPassedToAllowedNames), []);

    public static InvalidValuesSortingEnum NullContainsInAllowedNames = new(nameof(NullContainsInAllowedNames), [nameof(NullPassedToPropertyName), null ]);

    public static InvalidValuesSortingEnum EmptyStringContainsInAllowedNames = new(nameof(EmptyStringContainsInAllowedNames), [nameof(EmptyStringContainsInAllowedNames), string.Empty]);
    private InvalidValuesSortingEnum(string propertyName, IReadOnlyCollection<string> allowedNames) : base(propertyName, allowedNames)
    {
    }
}
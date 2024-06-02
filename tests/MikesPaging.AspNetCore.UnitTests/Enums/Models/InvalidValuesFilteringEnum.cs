using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;

namespace MikesPaging.AspNetCore.UnitTests.Enums.Models;

public class InvalidValuesFilteringEnum : FilteringEnum
{
    public static InvalidValuesFilteringEnum DuplicatesInAllowedNames = new(nameof(DuplicatesInAllowedNames), [nameof(DuplicatesInAllowedNames), nameof(DuplicatesInAllowedNames)]);

    public static InvalidValuesFilteringEnum NullPassedToPropertyName = new(null, [nameof(NullPassedToPropertyName)]);

    public static InvalidValuesFilteringEnum EmptyStringPassedToPropertyName = new(string.Empty, [nameof(EmptyStringPassedToPropertyName)]);

    public static InvalidValuesFilteringEnum NullPassedToAllowedNames = new(nameof(NullPassedToAllowedNames), null);

    public static InvalidValuesFilteringEnum EmptyCollectionPassedToAllowedNames = new(nameof(EmptyCollectionPassedToAllowedNames), []);

    public static InvalidValuesFilteringEnum NullContainsInAllowedNames = new(nameof(NullContainsInAllowedNames), [nameof(NullPassedToPropertyName), null]);

    public static InvalidValuesFilteringEnum EmptyStringContainsInAllowedNames = new(nameof(EmptyStringContainsInAllowedNames), [nameof(EmptyStringContainsInAllowedNames), string.Empty]);

    public static InvalidValuesFilteringEnum DuplicatesInInapplicableOperators = 
        new(nameof(EmptyStringContainsInAllowedNames), [nameof(EmptyStringContainsInAllowedNames), string.Empty], inapplicableOperators: [ FilteringOperators.NotEqual, FilteringOperators.NotEqual ]);

    private InvalidValuesFilteringEnum(string propertyName, IReadOnlyCollection<string> allowedNames, IReadOnlyCollection<FilteringOperators>? inapplicableOperators = null) 
        : base(propertyName, allowedNames, inapplicableOperators)
    {
    }
}

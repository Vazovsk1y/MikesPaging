using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;

namespace MikesPaging.AspNetCore;

internal static class Errors
{
    internal static string ValueCannotBeNullOrEmpty(string propertyName) => $"'{propertyName}' cannot be null or empty.";

    internal static string ValueCannotBeNull(string propertyName) => $"'{propertyName}' cannot be null.";

    internal static string InvalidStringValue(string propertyName, string value) => $"Invalid '{propertyName}' value: '{value}'.";

    internal static class Paging
    {
        public const string PageSizeMustBeGreaterThanZero = "Page size must be greater than zero.";
        public const string PageIndexMustBeGreaterThanZero = "Page index must be greater than zero.";
        public const string TotalItemsCountCannotBeLowerThanCurrentItemsCount = "Total items count cannot be lower than current items count.";
        public const string TotalItemsCountMustBeEqualToCurrentItemsCount = "Total items count must be equal to current items count.";
    }

    internal static class Filtering
    {
        public const string FiltersCollectionCannotContainDuplicates = "Filters collection cannot contain duplicates.";
        public static string OperatorIsNotApplicableFor(FilteringEnum filteringEnum, FilteringOperators @operator) => $"'{@operator}' is not applicable operator for '{filteringEnum.PropertyName}' property.";
    }
}

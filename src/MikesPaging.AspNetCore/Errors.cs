namespace MikesPaging.AspNetCore;

internal static class Errors
{
    internal static class Paging
    {
        public const string PageSizeMustBeGreaterThanZero = "Page size must be greater than zero.";
        public const string PageIndexMustBeGreaterThanZero = "Page index must be greater than zero.";
        public const string TotalItemsCountCannotBeLowerThanCurrentItemsCount = "Total items count cannot be lower than current items count.";
        public const string TotalItemsCountMustBeEqualToCurrentItemsCount = "Total items count must be equal to current items count.";
    }
}

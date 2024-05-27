namespace MikesPaging.AspNetCore.Common.ViewModels;

/// <summary>
/// Represents the sorting options model that can be used like endpoint argument.
/// </summary>
/// <param name="SortDirection">The direction of sorting (ascending or descending).</param>
/// <param name="SortBy">The field or property by which to sort.</param>
public record SortingOptionsModel(string SortDirection, string SortBy);
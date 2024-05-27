namespace MikesPaging.AspNetCore.Common.ViewModels;

/// <summary>
/// Represents filtering options including a collection of filters and the logic to apply them that can be used like endpoint argument.
/// </summary>
/// <param name="Filters">The collection of filters to be applied.</param>
/// <param name="Logic">The logic to be used for combining the filters (e.g., "AND", "OR").</param>
public record FilteringOptionsModel(IReadOnlyCollection<FilterModel> Filters, string Logic);

/// <summary>
/// Represents a single filter to be applied in filtering operations.
/// </summary>
/// <param name="FilterBy">The field or property by which to filter.</param>
/// <param name="Value">The value to filter by. This can be null.</param>
/// <param name="Operator">The operator to use for filtering (e.g., "Equals", "Contains" etc.).</param>
public record FilterModel(string FilterBy, string? Value, string Operator);


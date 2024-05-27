namespace MikesPaging.AspNetCore.Common.ViewModels;

/// <summary>
/// Represents the paging options that can be used like endpoint argument.
/// </summary>
/// <param name="PageIndex">The one-based index of the current page.</param>
/// <param name="PageSize">The number of items per page.</param>
public record PagingOptionsModel(int PageIndex, int PageSize);

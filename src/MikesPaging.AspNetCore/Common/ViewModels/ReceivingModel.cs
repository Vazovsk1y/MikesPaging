namespace MikesPaging.AspNetCore.Common.ViewModels;

/// <summary>
/// Represents the model used for receiving data, including paging, sorting, and filtering options. This can be used like endpoint argument.
/// </summary>
/// <param name="PagingOptions">The paging options.</param>
/// <param name="SortingOptions">The sorting options.</param>
/// <param name="FilteringOptions">The filtering options.</param>
public record ReceivingModel(PagingOptionsModel PagingOptions, SortingOptionsModel SortingOptions, FilteringOptionsModel FilteringOptions);

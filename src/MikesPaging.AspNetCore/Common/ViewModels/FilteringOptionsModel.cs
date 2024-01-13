namespace MikesPaging.AspNetCore.Common.ViewModels;

public record FilteringOptionsModel(IReadOnlyCollection<FilterModel> Filters, string Logic);
public record FilterModel(string FilterBy, string Value, string Operator);


using MikesPaging.AspNetCore.Common.Enums;

namespace MikesPaging.AspNetCore.Common;

public record Filter<TFilterBy>(TFilterBy FilterBy, FilteringOperators Operator, string Value);

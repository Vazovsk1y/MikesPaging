using MikesPaging.Core.Common.Enums;

namespace MikesPaging.Core.Common;

public record Filter<TFilterBy>(TFilterBy FilterBy, FilteringOperators Operator, string Value);

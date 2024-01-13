﻿using MikesPaging.AspNetCore.Common.Enums;
using System.Linq.Expressions;

namespace MikesPaging.AspNetCore.Common.Interfaces;

public interface IFilteringConfiguration<TSource, TFilterBy>
    where TFilterBy : Enum
{
    IReadOnlyDictionary<FilterKey<TFilterBy>, Func<string, Expression<Func<TSource, bool>>>> Filters { get; }
}
public record FilterKey<T>(T FilterBy, FilteringOperators Operator)
    where T : Enum;
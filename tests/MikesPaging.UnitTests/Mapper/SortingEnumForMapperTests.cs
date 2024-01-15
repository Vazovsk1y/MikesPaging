﻿using MikesPaging.AspNetCore.Common;
using MikesPaging.UnitTests.Models;

namespace MikesPaging.UnitTests.Mapper;

public class SortingEnumForMapperTests : SortingEnum
{
    public static readonly SortingEnumForMapperTests ByFirstName = new(nameof(TestEntity.FirstName), AllowedTestEntityNames.AllowedNamesForFirstName);

    public static readonly SortingEnumForMapperTests ByLastName = new(nameof(TestEntity.LastName), AllowedTestEntityNames.AllowedNamesForLastName);

    public static readonly SortingEnumForMapperTests ByCreated = new(nameof(TestEntity.Created), AllowedTestEntityNames.AllowedNamesForCreated);

    public static readonly SortingEnumForMapperTests ByAge = new(nameof(TestEntity.Age), AllowedTestEntityNames.AllowedNamesForAge);

    private SortingEnumForMapperTests(string propertyName, IReadOnlyCollection<string> allowedNames) : base(propertyName, allowedNames)
    {
    }
}
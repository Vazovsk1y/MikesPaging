﻿using MikesPaging.AspNetCore.UnitTests.Models;

namespace MikesPaging.AspNetCore.UnitTests;

internal static class Data
{
    public static readonly TestEntity[] TestEntities =
    [
       new() { Id = Guid.NewGuid(), Age = 2, Created = DateTimeOffset.UtcNow, FirstName = "John", LastName = "Doe", IQ = 5 },
       new() { Id = Guid.NewGuid(), Age = 3, Created = DateTimeOffset.UtcNow, FirstName = "Mike", LastName = "Vazovskiy", IQ = null },
       new() { Id = Guid.NewGuid(), Age = 1, Created = DateTimeOffset.UtcNow, FirstName = "Dr.", LastName = "ForNever", IQ = 8 }
    ];
}

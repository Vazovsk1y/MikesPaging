using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.UnitTests.Models;

namespace MikesPaging.AspNetCore.UnitTests.Mapper;

public class FilteringEnumForMapperTests : FilteringEnum
{
    public static readonly FilteringEnumForMapperTests ByFirstName = new(nameof(TestEntity.FirstName), AllowedTestEntityNames.AllowedNamesForFirstName);

    public static readonly FilteringEnumForMapperTests ByLastName = new(nameof(TestEntity.LastName), AllowedTestEntityNames.AllowedNamesForLastName);

    public static readonly FilteringEnumForMapperTests ByCreated = new(nameof(TestEntity.Created), AllowedTestEntityNames.AllowedNamesForCreated);

    public static readonly FilteringEnumForMapperTests ByAge = new(nameof(TestEntity.Age), AllowedTestEntityNames.AllowedNamesForAge);

    public static readonly FilteringEnumForMapperTests ByAnyPropertyWithInapplicableOperators = 
        new(nameof(ByAnyPropertyWithInapplicableOperators), [nameof(ByAnyPropertyWithInapplicableOperators)], inapplicableOperators: [ FilteringOperators.GreaterThan, FilteringOperators.NotEqual ]);

    internal FilteringEnumForMapperTests(string propertyName, IReadOnlyCollection<string> allowedNames, bool ignoreCase = true, IReadOnlyCollection<FilteringOperators>? inapplicableOperators = null) : base(propertyName, allowedNames, ignoreCase, inapplicableOperators)
    {
    }
}
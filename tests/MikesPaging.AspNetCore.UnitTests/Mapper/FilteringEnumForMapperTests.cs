using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.UnitTests.Models;

namespace MikesPaging.AspNetCore.UnitTests.Mapper;

public class FilteringEnumForMapperTests : FilteringEnum
{
    public static readonly FilteringEnumForMapperTests ByFirstName = new(nameof(TestEntity.FirstName), AllowedTestEntityNames.AllowedNamesForFirstName);

    public static readonly FilteringEnumForMapperTests ByLastName = new(nameof(TestEntity.LastName), AllowedTestEntityNames.AllowedNamesForLastName);

    public static readonly FilteringEnumForMapperTests ByCreated = new(nameof(TestEntity.Created), AllowedTestEntityNames.AllowedNamesForCreated);

    public static readonly FilteringEnumForMapperTests ByAge = new(nameof(TestEntity.Age), AllowedTestEntityNames.AllowedNamesForAge);

    private FilteringEnumForMapperTests(string propertyName, IReadOnlyCollection<string> allowedNames) : base(propertyName, allowedNames)
    {
    }
}
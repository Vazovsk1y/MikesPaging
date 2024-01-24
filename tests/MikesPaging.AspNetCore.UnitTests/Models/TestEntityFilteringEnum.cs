using MikesPaging.AspNetCore.Common;

namespace MikesPaging.AspNetCore.UnitTests.Models;

public class TestEntityFilteringEnum : FilteringEnum
{
    public static readonly TestEntityFilteringEnum ByFirstName = new(nameof(TestEntity.FirstName), AllowedTestEntityNames.AllowedNamesForFirstName);

    public static readonly TestEntityFilteringEnum ByLastName = new(nameof(TestEntity.LastName), AllowedTestEntityNames.AllowedNamesForLastName);

    public static readonly TestEntityFilteringEnum ByCreated = new(nameof(TestEntity.Created), AllowedTestEntityNames.AllowedNamesForCreated);

    public static readonly TestEntityFilteringEnum ByAge = new(nameof(TestEntity.Age), AllowedTestEntityNames.AllowedNamesForAge);

    public static readonly TestEntityFilteringEnum ByAgeButWithInvalidPropertyName = new("invalid", ["invalid"]);

    private TestEntityFilteringEnum(string propertyName, IReadOnlyCollection<string> allowedNames) : base(propertyName, allowedNames)
    {
    }
}
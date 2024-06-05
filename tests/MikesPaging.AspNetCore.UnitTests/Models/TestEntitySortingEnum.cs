using MikesPaging.AspNetCore.Common;

namespace MikesPaging.AspNetCore.UnitTests.Models;

public class TestEntitySortingEnum : SortingEnum
{
    public static readonly TestEntitySortingEnum ByFirstName = new(nameof(TestEntity.FirstName), AllowedTestEntityNames.AllowedNamesForFirstName);

    public static readonly TestEntitySortingEnum ByLastName = new(nameof(TestEntity.LastName), AllowedTestEntityNames.AllowedNamesForLastName);

    public static readonly TestEntitySortingEnum ByCreated = new(nameof(TestEntity.Created), AllowedTestEntityNames.AllowedNamesForCreated);

    public static readonly TestEntitySortingEnum ByAge = new(nameof(TestEntity.Age), AllowedTestEntityNames.AllowedNamesForAge);

    public static readonly TestEntitySortingEnum ByAgeButWithInvalidPropertyName = new("invalid", ["invalid"]);

    public static readonly TestEntitySortingEnum ByRelatedCollectionCount = new("RelatedCollectionCount", [ "RelatedCollectionCount" ]);

    private TestEntitySortingEnum(string propertyName, IReadOnlyCollection<string> allowedNames) : base(propertyName, allowedNames)
    {
    }
}